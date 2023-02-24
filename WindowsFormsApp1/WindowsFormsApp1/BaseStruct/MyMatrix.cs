using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    internal class MyMatrix
    {

        public MyFloat4 XPlane = new MyFloat4();
        public MyFloat4 YPlane = new MyFloat4();
        public MyFloat4 ZPlane = new MyFloat4();
        public MyFloat4 WPlane = new MyFloat4();

        public MyMatrix()
        {
            XPlane.X = 1;
            YPlane.Y = 1;
            ZPlane.Z = 1;
            WPlane.W = 1;
        }


        //public static MyFloat4 operator*(MyFloat4 Vector, MyMatrix Mat)
        //{
        //    MyFloat4 Ret = new MyFloat4();
        //    Ret.X = Vector.X * Mat.XPlane.X + Vector.Y * Mat.YPlane.X + Vector.Z * Mat.ZPlane.X + Vector.W * Mat.WPlane.X;
        //    Ret.Y = Vector.X * Mat.XPlane.Y + Vector.Y * Mat.YPlane.Y + Vector.Z * Mat.ZPlane.Y + Vector.W * Mat.WPlane.Y;
        //    Ret.Z = Vector.X * Mat.XPlane.Z + Vector.Y * Mat.YPlane.Z + Vector.Z * Mat.ZPlane.Z + Vector.W * Mat.WPlane.Z;
        //    Ret.W = Vector.X * Mat.XPlane.W + Vector.Y * Mat.YPlane.W + Vector.Z * Mat.ZPlane.W + Vector.W * Mat.WPlane.W;
        //    return Ret;
        //}


  
        public static MyMatrix operator *(MyMatrix A, MyMatrix B)
        {
            MyMatrix Ret = new MyMatrix();
            Ret.XPlane = A.XPlane * B;
            Ret.YPlane = A.YPlane * B;
            Ret.ZPlane = A.ZPlane * B;
            Ret.WPlane = A.WPlane * B;
            return Ret;
        }

        public MyFloat3 TransportDirection(MyFloat3 Vector)
        {
            MyFloat3 Ret = new MyFloat3();
            Ret.X = Vector.X * XPlane.X + Vector.Y * YPlane.X + Vector.Z * ZPlane.X;
            Ret.Y = Vector.X * XPlane.Y + Vector.Y * YPlane.Y + Vector.Z * ZPlane.Y;
            Ret.Z = Vector.X * XPlane.Z + Vector.Y * YPlane.Z + Vector.Z * ZPlane.Z;
            return Ret;
        }

        public MyMatrix GetTransposed()
        {
            MyMatrix Ret = new MyMatrix();
            Ret.XPlane = new MyFloat4(XPlane.X, YPlane.X, ZPlane.X, WPlane.X);
            Ret.YPlane = new MyFloat4(XPlane.Y, YPlane.Y, ZPlane.Y, WPlane.Y);
            Ret.ZPlane = new MyFloat4(XPlane.Z, YPlane.Z, ZPlane.Z, WPlane.Z);
            Ret.WPlane = new MyFloat4(XPlane.W, YPlane.W, ZPlane.W, WPlane.W);
            return Ret;
        }

        public MyMatrix GetInverse()
        {
            float[][] MatrixArr = new float[][]
            {
                new float[]{ XPlane.X, XPlane.Y, XPlane.Z, XPlane.W},
                new float[]{ YPlane.X, YPlane.Y, YPlane.Z, YPlane.W},
                new float[]{ ZPlane.X, ZPlane.Y, ZPlane.Z, ZPlane.W},
                new float[]{ WPlane.X, WPlane.Y, WPlane.Z, WPlane.W}
            };

            float[][] InverseMatrixArr = InverseMatrix(MatrixArr);

            MyMatrix Ret = new MyMatrix();
            Ret.XPlane = new MyFloat4(InverseMatrixArr[0][0], InverseMatrixArr[0][1], InverseMatrixArr[0][2], InverseMatrixArr[0][3]);
            Ret.YPlane = new MyFloat4(InverseMatrixArr[1][0], InverseMatrixArr[1][1], InverseMatrixArr[1][2], InverseMatrixArr[1][3]);
            Ret.ZPlane = new MyFloat4(InverseMatrixArr[2][0], InverseMatrixArr[2][1], InverseMatrixArr[2][2], InverseMatrixArr[2][3]);
            Ret.WPlane = new MyFloat4(InverseMatrixArr[3][0], InverseMatrixArr[3][1], InverseMatrixArr[3][2], InverseMatrixArr[3][3]);
            return Ret;
        }


        /// 求矩阵的逆矩阵
        private static float[][] InverseMatrix(float[][] matrix)
        {
            //matrix必须为非空
            if (matrix == null || matrix.Length == 0)
            {
                return new float[][] { };
            }

            //matrix 必须为方阵
            int len = matrix.Length;
            for (int counter = 0; counter < matrix.Length; counter++)
            {
                if (matrix[counter].Length != len)
                {
                    throw new Exception("matrix 必须为方阵");
                }
            }

            //计算矩阵行列式的值
            float dDeterminant = Determinant(matrix);
            if (Math.Abs(dDeterminant) <= 1E-6)
            {
                throw new Exception("矩阵不可逆");
            }

            //制作一个伴随矩阵大小的矩阵
            float[][] result = AdjointMatrix(matrix);

            //矩阵的每项除以矩阵行列式的值，即为所求
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    result[i][j] = result[i][j] / dDeterminant;
                }
            }

            return result;
        }

        // 递归计算行列式的值
        private static float Determinant(float[][] matrix)
        {
            //二阶及以下行列式直接计算
            if (matrix.Length == 0) return 0;
            else if (matrix.Length == 1) return matrix[0][0];
            else if (matrix.Length == 2)
            {
                return matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];
            }

            //对第一行使用“加边法”递归计算行列式的值
            float dSum = 0, dSign = 1;
            for (int i = 0; i < matrix.Length; i++)
            {
                float[][] matrixTemp = new float[matrix.Length - 1][];
                for (int count = 0; count < matrix.Length - 1; count++)
                {
                    matrixTemp[count] = new float[matrix.Length - 1];
                }

                for (int j = 0; j < matrixTemp.Length; j++)
                {
                    for (int k = 0; k < matrixTemp.Length; k++)
                    {
                        matrixTemp[j][k] = matrix[j + 1][k >= i ? k + 1 : k];
                    }
                }

                dSum += (matrix[0][i] * dSign * Determinant(matrixTemp));
                dSign = dSign * -1;
            }

            return dSum;
        }

        // 计算方阵的伴随矩阵
        private static float[][] AdjointMatrix(float[][] matrix)
        {
            //制作一个伴随矩阵大小的矩阵
            float[][] result = new float[matrix.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new float[matrix[i].Length];
            }

            //生成伴随矩阵
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result.Length; j++)
                {
                    //存储代数余子式的矩阵（行、列数都比原矩阵少1）
                    float[][] temp = new float[result.Length - 1][];
                    for (int k = 0; k < result.Length - 1; k++)
                    {
                        temp[k] = new float[result[k].Length - 1];
                    }

                    //生成代数余子式
                    for (int x = 0; x < temp.Length; x++)
                    {
                        for (int y = 0; y < temp.Length; y++)
                        {
                            temp[x][y] = matrix[x < i ? x : x + 1][y < j ? y : y + 1];
                        }
                    }

                    //Console.WriteLine("代数余子式:");
                    //PrintMatrix(temp);

                    result[j][i] = ((i + j) % 2 == 0 ? 1 : -1) * Determinant(temp);
                }
            }

            //Console.WriteLine("伴随矩阵：");
            //PrintMatrix(result);

            return result;
        }

        private static void PrintMatrix(float[][] matrix, string title = "")
        {
            //1.标题值为空则不显示标题
            if (!String.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title);
            }

            //2.打印矩阵
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    Console.Write(matrix[i][j] + "\t");
                    //注意不能写为：Console.Write(matrix[i][j] + '\t');
                }
                Console.WriteLine();
            }

            //3.空行
            Console.WriteLine();
        }


    }


}
