using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SPICA.Formats.H3D;
using SPICA.Formats.H3D.Model.Mesh;
using SPICA.WinForms.Rendering;

using System.Diagnostics;

namespace SPICA.WinForms
{
    public partial class Form1 : GameWindow
    {
        string vertexShaderSource = @"
#version 130
precision highp float;
uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;
in vec3 in_position;
in vec3 in_normal;
out vec3 normal;
void main(void)
{
    gl_Position = projection_matrix * modelview_matrix * vec4(in_position, 1);
  //works only for orthogonal modelview
  normal = (modelview_matrix * vec4(in_normal, 0)).xyz;
  
  
}";

        string fragmentShaderSource = @"
#version 130
precision highp float;
const vec3 ambient = vec3(0.1, 0.1, 0.1);
const vec3 lightVecNormalized = normalize(vec3(0, 100, 50));
const vec3 lightColor = vec3(1, 1, 1);
in vec3 normal;
out vec4 out_frag_color;
void main(void)
{
  float diffuse = clamp(dot(lightVecNormalized, normalize(normal)), 0.0, 1.0);
  out_frag_color = vec4(ambient + diffuse * lightColor, 1.0);
}";

        int vertexShaderHandle,
            fragmentShaderHandle,
            shaderProgramHandle,
            modelviewMatrixLocation,
            projectionMatrixLocation,
            vaoHandle,
            positionVboHandle,
            normalVboHandle,
            eboHandle;

        int[] PositionHandles;
        int[] NormalHandles;
        int[] VAOHandles;

        H3D Model;

        Matrix4 projectionMatrix, modelviewMatrix;

        public Form1()
            : base(640, 480,
            new GraphicsMode(), "OpenGL 3 Example", 0,
            DisplayDevice.Default, 3, 0,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        { }

        Mesh[] Mdl;

        protected override void OnLoad(System.EventArgs e)
        {
            VSync = VSyncMode.On;

            CreateShaders();

            H3D H3D = H3D.Open("D:\\may.bch");

            Debug.Write(H3D.Models[0].WorldTransform.ToString());
            Debug.Write(H3D.Models.Tree[1].Name + '\n');
            Debug.Write(H3D.Models[0].Name + '\n');

            //H3D.Save("D:\\recreated.bch", H3D);

            Mdl = new Mesh[H3D.Models[0].Meshes.Count];

            for (int Index = 0; Index < Mdl.Length; Index++)
            {
                H3DMesh Mesh = H3D.Models[0].Meshes[Index];

                ushort[][] Indices = new ushort[Mesh.SubMeshes.Count][];

                for (int SM = 0; SM < Mesh.SubMeshes.Count; SM++)
                {
                    Indices[SM] = Mesh.SubMeshes[SM].Indices;
                }

                Mdl[Index] = new Mesh(Mesh.Attributes, Mesh.RawBuffer, Indices, Mesh.VertexStride, shaderProgramHandle);
            }

            // Other state
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(System.Drawing.Color.MidnightBlue);
        }

        void CreateShaders()
        {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            Debug.WriteLine(GL.GetShaderInfoLog(vertexShaderHandle));
            Debug.WriteLine(GL.GetShaderInfoLog(fragmentShaderHandle));

            // Create program
            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(shaderProgramHandle);

            Debug.WriteLine(GL.GetProgramInfoLog(shaderProgramHandle));

            GL.UseProgram(shaderProgramHandle);

            // Set uniforms
            projectionMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "projection_matrix");
            modelviewMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "modelview_matrix");

            float aspectRatio = ClientSize.Width / (float)(ClientSize.Height);
            Matrix4.CreatePerspectiveFieldOfView((float)System.Math.PI / 4, aspectRatio, 1, 1000, out projectionMatrix);
            modelviewMatrix = Matrix4.LookAt(new Vector3(0, 60, 180), new Vector3(0, 60, 0), new Vector3(0, 1, 0));

            GL.UniformMatrix4(projectionMatrixLocation, false, ref projectionMatrix);
            GL.UniformMatrix4(modelviewMatrixLocation, false, ref modelviewMatrix);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Matrix4 rotation = Matrix4.CreateRotationY((float)e.Time);
            Matrix4.Mult(ref rotation, ref modelviewMatrix, out modelviewMatrix);
            GL.UniformMatrix4(modelviewMatrixLocation, false, ref modelviewMatrix);

            if (Keyboard[OpenTK.Input.Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (Mesh Mesh in Mdl) Mesh.Render();

            SwapBuffers();
        }
    }
}
