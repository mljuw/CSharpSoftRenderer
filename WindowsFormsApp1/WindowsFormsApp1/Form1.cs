
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1.BaseStruct;
using WindowsFormsApp1.Light;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        private MyRenderer Renderer;
        Graphics G;

        //string Path = System.Windows.Forms.Application.StartupPath;
        string Path = System.Environment.CurrentDirectory;

        enum SceneType
        {
            BlinnPhong,
            PBR,
            Wireframe,
            Shadow,
        }

        SceneType CurSceneType = SceneType.PBR;
        AntiAliasingType AAType = AntiAliasingType.None;

        bool bDisplayNormal = false;
        bool bDisplayShadowMap = false;

        public Form1()
        {
            InitializeComponent();
            G = pictureBox1.CreateGraphics();//创建一个画板

        }

        private void LoadWireframeScene()
        {

            Renderer = new MyRenderer();
            Renderer.SetPrespectiveProjection(90, 1, 1250, 2);
            Renderer.SetScreenSize(1440, 720);

            MaterialParams MatParams = new MaterialParams();
            var ModelPath =  "\\Models\\Sword\\Sword.OBJ";
            Transform Trans = new Transform();
            Trans.SetLocation(100, 0, 0);
            Trans.SetRotation(0, 0, 0);

            Renderer.AddModel(ModelPath, ShaderType.WireFrame, Trans, MatParams);
        }

        private void LoadBlinnPhongModel()
        {

            MaterialParams MatParams = new MaterialParams();
            MatParams.Ka = new MyFloat3(0.00f, 0.00f, 0.00f);
            MatParams.Diffuse = new Texture( "\\Models\\Sword\\MyDiffColor.PNG", TextureType.sRGB);
            MatParams.Specular = new Texture( "\\Models\\Sword\\MySpecColor.PNG", TextureType.LinearColor);
            MatParams.NormalMap = new Texture( "\\Models\\Sword\\Sword_NM.PNG", TextureType.Normal);


            /* 加载剑模型 */
            //var ModelPath =  "\\Models\\Cat.OBJ";
            var ModelPath =  "\\Models\\Sword\\Sword.OBJ";
            Transform Trans = new Transform();
            Trans.SetLocation(100, 0, 0);
            Trans.SetRotation(0, 0, 0);


            Renderer.AddModel(ModelPath, ShaderType.BlinnPhong, Trans, MatParams);

        }

        private void LoadPanelModel()
        {
            MaterialParams MatParams = new MaterialParams();

            var ModelPath =  "\\Models\\PlaneMesh.OBJ";
            Transform Trans = new Transform();
            Trans.SetLocation(100, -120, -120);
            Trans.SetRotation(0, -90, 0);
            Trans.SetScale(3, 3, 3);


            Renderer.AddModel(ModelPath, ShaderType.BlinnPhong, Trans, MatParams);
        }

        private void LoadShadowScene()
        {
            Renderer = new MyRenderer();
            Renderer.SetPrespectiveProjection(90, 1, 1250, 2);
            Renderer.SetScreenSize(1440, 720);
            Renderer.DisplayShadowMapping = true;

            Renderer.GetShaderGlobal().AmbientColor = new MyFloat3(0.35f, 0.35f, 0.35f);

            LoadBlinnPhongModel();
            LoadPanelModel();

            DirectionalLight DirectionalLight = new DirectionalLight();
            DirectionalLight.Transform.SetLocation(0, 0, 300);
            DirectionalLight.Transform.SetRotation(0, -55, -20);
            DirectionalLight.SetColor(1, 1, 1);
            Renderer.AddLight(DirectionalLight);
        }

        private void LoadBlinnPhongScene()
        {
            Renderer = new MyRenderer();
            Renderer.SetPrespectiveProjection(90, 1, 1250, 2);
            Renderer.SetScreenSize(1440, 720);

            Renderer.GetShaderGlobal().AmbientColor = new MyFloat3(0.35f, 0.35f, 0.35f);

            LoadBlinnPhongModel();

            PointLight PointLight = new PointLight();
            PointLight.Transform.SetLocation(0, 0, 0);
            PointLight.SetColor(1, 1, 1);
            PointLight.SetSourceRange(50);
            PointLight.SetAttenuationRange(350);
            Renderer.AddLight(PointLight);

        }

        private void LoadPBRScene()
        {
            Renderer = new MyRenderer();
            Renderer.SetPrespectiveProjection(90, 1, 1250, 2);
            Renderer.SetScreenSize(1440, 720);

            Renderer.GetShaderGlobal().AmbientColor = new MyFloat3(0.35f, 0.35f, 0.35f);


            /* PBR 模型*/
            LoadPBRModel1();
            LoadPBRModel2();


            PointLight PointLight = new PointLight();
            PointLight.Transform.SetLocation(-350, -150, -50);
            PointLight.SetColor(1, 1, 1);
            PointLight.SetSourceRange(50);
            PointLight.SetAttenuationRange(5000);


            PointLight PointLight2 = new PointLight();
            PointLight2.Transform.SetLocation(-150, 150, 80);
            PointLight2.SetColor(1, 1, 1);
            PointLight2.SetSourceRange(50);
            PointLight2.SetAttenuationRange(5000);

            Renderer.AddLight(PointLight);
            Renderer.AddLight(PointLight2);
        }

        private void LoadPBRModel1()
        {

            MaterialParams MatParams = new MaterialParams();
            MatParams.PBRParams.AO = new Texture( "\\Models\\PBR\\Model1\\T_AO.PNG", TextureType.LinearColor);
            MatParams.PBRParams.BaseColor = new Texture( "\\Models\\PBR\\Model1\\T_BaseColor.PNG", TextureType.sRGB);
            MatParams.PBRParams.Emissive = new Texture( "\\Models\\PBR\\Model1\\T_Emissive.PNG", TextureType.sRGB);
            MatParams.PBRParams.Metalic = new Texture( "\\Models\\PBR\\Model1\\T_Metalic.PNG", TextureType.LinearColor);
            MatParams.PBRParams.Roughness = new Texture( "\\Models\\PBR\\Model1\\T_Roughness.PNG", TextureType.LinearColor);
            MatParams.NormalMap = new Texture( "\\Models\\PBR\\Model1\\T_Weapon_Set1_Normal.PNG", TextureType.Normal);


            var ModelPath =  "\\Models\\PBR\\Model1\\SM_Weapon.OBJ";
            Transform Trans = new Transform();
            Trans.SetLocation(22, -10, -10);
            Trans.SetRotation(0, 90, 0);

            //var ModelPath =  "\\Models\\Sphere.OBJ";
            //Transform Trans = new Transform();
            //Trans.SetLocation(new MyFloat3(100, 0, 0));
            //Trans.SetRotation(new MyFloat3(0, 0, 0));

            Renderer.AddModel(ModelPath, ShaderType.PBR, Trans, MatParams);
        }

        private void LoadPBRModel2()
        {

            MaterialParams MatParams = new MaterialParams();
            MatParams.PBRParams.RMSCombine = new Texture( "\\Models\\PBR\\Model2\\T_Hero_weapon_27_S.PNG", TextureType.LinearColor);
            MatParams.PBRParams.BaseColor = new Texture( "\\Models\\PBR\\Model2\\T_Hero_weapon_27_D.PNG", TextureType.sRGB);
            MatParams.NormalMap = new Texture( "\\Models\\PBR\\Model2\\T_Hero_weapon_27_N.PNG", TextureType.Normal);


            var ModelPath =  "\\Models\\PBR\\Model2\\SM_Hero27_Weapon2.OBJ";
            Transform Trans = new Transform();
            Trans.SetLocation(22, -15, 10);
            Trans.SetRotation(0, 90, 0);

            //var ModelPath =  "\\Models\\Sphere.OBJ";
            //Transform Trans = new Transform();
            //Trans.SetLocation(new MyFloat3(100, 0, 0));
            //Trans.SetRotation(new MyFloat3(0, 0, 0));

            Renderer.AddModel(ModelPath, ShaderType.PBR, Trans, MatParams);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CurSceneType = SceneType.Wireframe;
            DoRender();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CurSceneType = SceneType.BlinnPhong;
            DoRender();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CurSceneType = SceneType.PBR;
            DoRender();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CurSceneType = SceneType.Shadow;
            DoRender();
        }

        private void DoRender()
        {
            switch (CurSceneType)
            {
                case SceneType.PBR:
                    LoadPBRScene();
                    break;
                case SceneType.Wireframe:
                    LoadWireframeScene();
                    break;
                case SceneType.BlinnPhong:
                    LoadBlinnPhongScene();
                    break;
                case SceneType.Shadow:
                    LoadShadowScene();
                    Renderer.DisplayShadowMapping = bDisplayShadowMap;
                    break;
            }


            Renderer.DisplayNormal = bDisplayNormal;
            Renderer.DisplayTangent = bDisplayNormal;
            Renderer.DisplayBiTangent = bDisplayNormal;
            Renderer.SetAntiAlising(AAType);
            Renderer.BeginPipeline();
            Renderer.Render(G);
        }

        private void Rdo_NoneAA_CheckedChanged(object sender, EventArgs e)
        {
            if (Rdo_NoneAA.Checked)
            {
                AAType = AntiAliasingType.None;
                DoRender();
            }
        }

        private void Rdo_SSAA_CheckedChanged(object sender, EventArgs e)
        {
            if (Rdo_SSAA.Checked)
            {
                AAType = AntiAliasingType.SSAA;
                DoRender();
            }
        }

        private void Rdo_MSAA_CheckedChanged(object sender, EventArgs e)
        {
            if (Rdo_MSAA.Checked)
            {
                AAType = AntiAliasingType.MSAA;
                DoRender();
            }
        }

        private void Chk_ShowNormal_CheckedChanged(object sender, EventArgs e)
        {
            bDisplayNormal = Chk_ShowNormal.Checked;
            DoRender();
        }

        private void Chk_ShowShadowMap_CheckedChanged(object sender, EventArgs e)
        {
            bDisplayShadowMap = Chk_ShowShadowMap.Checked;
            DoRender();
        }
    }
}
