using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp1
{
    internal class ObjPart
    {
        /// <summary>
        /// 材质名称
        /// </summary>
        public string strMatName;

        /// <summary>
        /// UV坐标数组
        /// </summary>
        public List<MyFloat2> listUV;

        /// <summary>
        /// 法线数组
        /// </summary>
        public List<MyFloat3> listNormal;

        /// <summary>
        /// 切线数组
        /// </summary>
        public List<MyFloat4> listTangent;

        /// <summary>
        /// 顶点数组
        /// </summary>
        public List<MyFloat3> listVertex;

        /// <summary>
        /// 面数组
        /// </summary>
        public List<int> listTriangle;

        public ObjPart()
        {
            strMatName = "";
            listUV = new List<MyFloat2>();
            listNormal = new List<MyFloat3>();
            listVertex = new List<MyFloat3>();
            listTriangle = new List<int>();
            listTangent = new List<MyFloat4>();
        }
    }

    internal class ObjMatItem
    {
        /// <summary>
        /// 材质名称
        /// </summary>
        public string strMatName;
        public int illum;
        public MyFloat3 Kd;
        public MyFloat3 Ka;
        public MyFloat3 Tf;
        public MyFloat2 widthHeight = new MyFloat2();
        public string map_Kd;
        public float Ni;

        public ObjMatItem()
        {
            strMatName = "";
            illum = -1;
            Kd = new MyFloat3(0.0f, 0.0f, 0.0f);
            Ka = new MyFloat3(0.0f, 0.0f, 0.0f);
            Tf = new MyFloat3(0.0f, 0.0f, 0.0f);
            map_Kd = "";
            Ni = 1.0f;
            widthHeight.X = 0.0f;
            widthHeight.Y = 0.0f;
        }
    }

    internal struct ObjModel
    {
        public List<ObjPart> objParts;
        public List<ObjMatItem> ObjMats;
    }

    internal class ObjMesh
    {
        /// <summary>
        /// UV坐标列表
        /// </summary>
        private List<MyFloat3> uvArrayList;

        /// <summary>
        /// 法线列表
        /// </summary>
        private List<MyFloat3> normalArrayList;

        /// <summary>
        /// 顶点列表
        /// </summary>
        private List<MyFloat3> vertexArrayList;

        public List<ObjPart> listObjParts;
        public List<ObjMatItem> listObjMats;

        public string _strMatPath { get; set; }
        public string _strObjPath { get; set; }
        public string _strObjName { get; set; }

        /// <summary>
        /// 构造函数    
        /// </summary>
        public ObjMesh()
        {
            //初始化列表
            uvArrayList = new List<MyFloat3>();
            normalArrayList = new List<MyFloat3>();
            vertexArrayList = new List<MyFloat3>();

            listObjParts = new List<ObjPart>();
            listObjMats = new List<ObjMatItem>();
            _strMatPath = _strObjName = "";
        }


        /// <summary>
        /// 从一个文本化后的.obj文件中加载模型
        /// </summary>
        public ObjMesh LoadFromObj(string strObjPath)
        {
            _strObjPath = strObjPath;
            _strObjName = strObjPath;
            _strObjName = _strObjName.Replace("/", "");
            //读取内容
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(strObjPath, Encoding.Default);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            string objText = reader.ReadToEnd();
            reader.Close();
            if (objText.Length <= 0)
                return null;

            //v这一行在3dsMax中导出的.obj文件
            //  前面是两个空格后面是一个空格
            objText = objText.Replace("  ", " ");

            //将文本化后的obj文件内容按行分割
            string[] allLines = objText.Split('\n');
            foreach (string line in allLines)
            {
                //将每一行按空格分割
                char[] charsToTrim = { ' ' };
                string[] chars = line.TrimEnd('\r').TrimStart(' ').Split(charsToTrim, StringSplitOptions.RemoveEmptyEntries);
                if (chars.Length <= 0)
                {
                    continue;
                }
                //根据第一个字符来判断数据的类型
                switch (chars[0])
                {
                    case "mtllib":
                        var FullPath = Path.GetFullPath(strObjPath);
                        var FileName = Path.GetFileName(strObjPath);
                        var FilePath = FullPath.Replace(FileName, "");
                        _strMatPath = FilePath + chars[1];
                        //_strMatPath = _strObjPath.Substring(0, _strObjPath.LastIndexOf('/') + 1) + chars[1];
                        break;
                    case "v":
                        //处理顶点
                        this.vertexArrayList.Add(new MyFloat3(
                            (ConvertToFloat(chars[1])),
                            ConvertToFloat(chars[2]),
                            ConvertToFloat(chars[3]))
                        );
                        break;
                    case "vn":
                        //处理法线
                        this.normalArrayList.Add(new MyFloat3(
                            ConvertToFloat(chars[1]),
                            ConvertToFloat(chars[2]),
                            ConvertToFloat(chars[3]))
                        );
                        break;
                    case "vt":
                        //处理UV
                        this.uvArrayList.Add(new MyFloat3(
                            ConvertToFloat(chars[1]),
                            ConvertToFloat(chars[2]))
                        );
                        break;
                    case "usemtl":
                        ObjPart objPart = new ObjPart();
                        objPart.strMatName = chars[1];//材质名称
                        listObjParts.Add(objPart);
                        break;
                    case "f":
                        //处理面
                        GetTriangleList(chars);
                        break;
                }
            }

            //获取mtl文件路径
            // string mtlFilePath = strObjPath.Replace(".obj", ".mtl");
            if (_strMatPath != "")
            {
                LoadMat(_strMatPath);
            }

            return this;
        }

        private void LoadMat(string strmtlPath)
        {
            //从mtl文件中加载材质
            listObjMats = ObjMaterial.Instance.LoadFormMtl(strmtlPath);
        }

        /// <summary>
        /// 获取面列表.
        /// </summary>
        /// <param name="chars">Chars.</param>
        /// 
        private List<MyFloat3> indexVectorList = new List<MyFloat3>();
        private MyFloat3 indexVector = new MyFloat3(0, 0);
        private void GetTriangleList(string[] chars)
        {
            if(listObjParts.Count <= 0)
            {
                listObjParts.Add(new ObjPart());
            }

            indexVectorList.Clear();
            for (int i = 1; i < chars.Length; ++i)
            {
                //将每一行按照空格分割后从第一个元素开始
                //按照/继续分割可依次获得顶点索引、法线索引和UV索引
                string[] indexs = chars[i].Split('/');
                MyFloat3 vertex = (MyFloat3)vertexArrayList[ConvertToInt(indexs[0]) - 1];
                listObjParts[listObjParts.Count - 1].listVertex.Add(vertex);//加入当前part的顶点

                indexVector = new MyFloat3(0, 0);
                //UV索引
                if (indexs.Length > 1)
                {
                    if (indexs[1] != "")
                        indexVector.Y = ConvertToInt(indexs[1]);
                }

                //法线索引
                if (indexs.Length > 2)
                {
                    if (indexs[2] != "")
                        indexVector.Z = ConvertToInt(indexs[2]);
                }

                //给UV数组赋值
                if (uvArrayList.Count > 0 && indexVector.Y > 0.01)
                {
                    MyFloat3 tVec = (MyFloat3)uvArrayList[(int)indexVector.Y - 1];
                    listObjParts[listObjParts.Count - 1].listUV.Add(new MyFloat2(tVec.X, tVec.Y));
                }

                //给法线数组赋值
                if (normalArrayList.Count > 0 && indexVector.Z > 0.01)
                {
                    MyFloat3 nVec = (MyFloat3)normalArrayList[(int)indexVector.Z - 1];
                    listObjParts[listObjParts.Count - 1].listNormal.Add(nVec);
                }

                //将索引向量加入列表中
                indexVectorList.Add(indexVector);
            }

            //面索引
            int nCount = listObjParts[listObjParts.Count - 1].listVertex.Count - indexVectorList.Count;
            for (int j = 1; j < indexVectorList.Count - 1; ++j)
            {
                //按照0,1,2这样的方式来组成面          
                listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount);
                listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount + j);
                listObjParts[listObjParts.Count - 1].listTriangle.Add(nCount + j + 1);
            }
        }

        /// <summary>
        /// 将一个字符串转换为浮点类型
        /// </summary>
        /// <param name="s">待转换的字符串</param>
        /// <returns></returns>
        private float ConvertToFloat(string s)
        {
            //return (float)System.Convert.ToDouble(s,CultureInfo.InvariantCulture);
            float fValue = 0.0f;
            try
            {
                fValue = (float)System.Convert.ToDouble(s);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fValue;
        }

        /// <summary>
        /// 将一个字符串转化为整型 /// </summary>
        /// <returns>待转换的字符串</returns>
        /// <param name="s"></param>
        private int ConvertToInt(string s)
        {
            //return System.Convert.ToInt32(s, CultureInfo.InvariantCulture);
            int nValue = 0;
            try
            {
                nValue = System.Convert.ToInt32(s);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return nValue;
        }

    }


    internal class ObjMaterial
    {
        /// <summary>
        /// 全局变量
        /// </summary>
        //private Texture globalTexture;

        /// <summary>
        /// 当前实例
        /// </summary>
        private static ObjMaterial instance;
        public static ObjMaterial Instance
        {
            get
            {
                if (instance == null)
                    instance = new ObjMaterial();//GameObject.FindObjectOfType<ObjMaterial>();
                return instance;
            }
        }

        /// <summary>
        /// 从一个文本化后的mtl文件加载一组材质
        /// </summary>
        /// <param name="mtlText">文本化的mtl文件</param>
        /// <param name="texturePath">贴图文件夹路径</param>
        public List<ObjMatItem> LoadFormMtl(string strMtlText)
        {
            List<ObjMatItem> listObjMats = new List<ObjMatItem>();
            StreamReader reader = new StreamReader(strMtlText, Encoding.Default);
            string mtlText = reader.ReadToEnd();
            reader.Close();

            if (mtlText == "")
                return listObjMats;

            //将文本化后的内容按行分割
            string[] allLines = mtlText.Split('\n');
            foreach (string line in allLines)
            {
                //按照空格分割每一行的内容
                string[] chars = line.TrimEnd('\r').TrimStart(' ').Split(' ');
                switch (chars[0])
                {
                    case "newmtl":
                        //处理材质名
                        ObjMatItem matItem = new ObjMatItem();
                        matItem.strMatName = chars[1];
                        listObjMats.Add(matItem);
                        break;
                    case "Ka":
                        listObjMats[listObjMats.Count - 1].Ka = new MyFloat3(
                            ConvertToFloat(chars[1]),
                            ConvertToFloat(chars[2]),
                            ConvertToFloat(chars[3])
                            );
                        break;
                    case "Kd":
                        //处理漫反射
                        listObjMats[listObjMats.Count - 1].Kd = new MyFloat3(
                            ConvertToFloat(chars[1]),
                            ConvertToFloat(chars[2]),
                            ConvertToFloat(chars[3])
                            );
                        break;
                    case "Ks":
                        //暂时仅考虑漫反射
                        break;
                    case "Ke":
                        //Todo
                        break;
                    case "Tf":
                        //处理漫反射
                        listObjMats[listObjMats.Count - 1].Tf = new MyFloat3(
                            ConvertToFloat(chars[1]),
                            ConvertToFloat(chars[2]),
                            ConvertToFloat(chars[3])
                            );
                        break;
                    case "Ni":
                        listObjMats[listObjMats.Count - 1].Ni = ConvertToFloat(chars[1]);
                        break;
                    case "e":
                        //Todo
                        break;
                    case "illum":
                        listObjMats[listObjMats.Count - 1].illum = Convert.ToInt32(chars[1]);
                        break;
                    case "map_Ka":
                        //暂时仅考虑漫反射
                        break;
                    case "map_Kd":
                        //处理漫反射贴图
                        string textureName = chars[1].Substring(chars[1].LastIndexOf("\\") + 1, chars[1].Length - chars[1].LastIndexOf("\\") - 1);
                        listObjMats[listObjMats.Count - 1].map_Kd = textureName;
                        break;
                    case "map_Ks":
                        //暂时仅考虑漫反射
                        break;
                }
            }

            return listObjMats;
        }

        /// <summary>
        /// 将一个字符串转换为浮点类型
        /// </summary>
        /// <param name="s">待转换的字符串</param>
        /// <returns></returns>
        private float ConvertToFloat(string s)
        {
            return System.Convert.ToSingle(s);
        }


    }
}
