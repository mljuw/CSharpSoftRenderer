# 软渲染器
用C# 0至1 实现了软渲染器（不基于OpenGL或DX渲染库）,模拟了显卡中的渲染管线流程, 除了解析obj模型文件的部分代码参考了一个博客文章外其余都是自己一行行代码实现.
主要有实现内容：

几何阶段：

&nbsp;&nbsp;  0.画线段

&nbsp;&nbsp;  1.顶点变换(VS)
  
&nbsp;&nbsp;  2.三角面裁剪与图形装配
  
&nbsp;&nbsp;  3.透视除法与屏幕空间映射
  
光栅化阶段：

&nbsp;&nbsp;  1.计算三角形包围盒
  
&nbsp;&nbsp;  2.判断三角形所影响的像素
  
&nbsp;&nbsp;  3.三角形重心坐标插值与透视矫正
  
&nbsp;&nbsp;  4.Early-Z
  
&nbsp;&nbsp;  5.片元着色器属性插值
  
&nbsp;&nbsp;  6.计算片元颜色(PS)

光照模型：

&nbsp;&nbsp;  1.BlinnPhong
  
&nbsp;&nbsp;  2.PBR
 
 灯光：
 
&nbsp;&nbsp;  1.平行光源
  
&nbsp;&nbsp;  2.点光源

抗锯齿：
&nbsp;&nbsp; SSAA
&nbsp;&nbsp; MSAA

目前只支持平行光源阴影.

&nbsp;&nbsp; 年末项目被裁后用2月时间在家做一些自己喜欢的事情，这个渲染器用了2周时间实现（主要回顾以前学过的知识），已经有快10年没用C#,利用目前这台电脑上已有的开发环境所以就用了c#, 代码里很多 new 操作还有拆箱与装箱问题，因为太久没用C#且时间太紧张就没做处理主要实现功能为主，之前时间都花在做 MOBA 的手游DEMO了，后面有时间会补充 内容有：

1.聚光灯
2.体积光
3.Bloom
4.ToonMapping
5.TAA
6.延时渲染
等等...

具体截图如下：

![image](https://github.com/mljuw/CSharpSoftRenderer/blob/master/Public/Screenshot/1wireframe.png)
![image](https://github.com/mljuw/CSharpSoftRenderer/blob/master/Public/Screenshot/2wireframenormal.png)
![image](https://github.com/mljuw/CSharpSoftRenderer/blob/master/Public/Screenshot/3BlinnPhong.jpg)
![image](https://github.com/mljuw/CSharpSoftRenderer/blob/master/Public/Screenshot/4PBR.jpg)
![image](https://github.com/mljuw/CSharpSoftRenderer/blob/master/Public/Screenshot/5Shadow.jpg)
![image](https://github.com/mljuw/CSharpSoftRenderer/blob/master/Public/Screenshot/6ShadowMap.jpg)
