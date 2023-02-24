# 软渲染器
用C# 实现了软渲染器，模拟了显卡中的 渲染管线流程，主要有实现内容：

几何阶段：

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

目前只支持平行光源阴影.


![image](https://github.com/mljuw/CSharpSoftRenderer/blob/master/Public/Screenshot/1wireframe.png)
