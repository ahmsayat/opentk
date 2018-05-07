/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenTK;
using OpenTK.Graphics;
*/


using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Windows;

using System.Windows.Controls;

using System.Windows.Data;

using System.Windows.Documents;

using System.Windows.Forms;

using System.Windows.Forms.Integration;

using System.Windows.Input;

using System.Windows.Media;

using System.Windows.Media.Imaging;

using System.Windows.Navigation;

using System.Windows.Shapes;


using OpenTK;

using OpenTK.Graphics;

using OpenTK.Graphics.OpenGL;


namespace dataArrows
{

    class Camera
    {
        private Vector3 target;
        private Vector3 origin;
        private float rotation;
        public Matrix4 ModelViewMatrix;

        public Camera(Vector3 target)
        {
            this.target = target;
            origin = target + new Vector3(5);
            SetMatrix();
        }

        public void Move(float x, float y, float z)
        {
            Move(new Vector3(x, y, z));
        }

        public void Move(Vector3 translation)
        {
            target += translation;
            origin += translation;
            SetMatrix();
        }

        public void Rotate(float deegres)
        {
            rotation += MathHelper.DegreesToRadians(deegres);
            SetMatrix();
        }

        public void Zoom(float zoom)
        {
            origin += new Vector3(zoom);
            SetMatrix();
        }

        public void SetMatrix()
        {
            var rotationMatrix = Matrix4.CreateFromAxisAngle(target, rotation); // rotation around target
            //var eye = Vector4.Transform(new Vector4(origin, 1), rotationMatrix);
            var eye = Vector4.Transform(new Vector4((origin - target), 1), rotationMatrix) + new Vector4(target, 1);
            ModelViewMatrix = Matrix4.LookAt(eye.Xyz, target, Vector3.UnitY);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GLControl glControl;
        int rotaX = 0;
        int rotaY = 0;
        Camera cam = new Camera(new Vector3(0, 0, 0));


        public MainWindow()
        {
            OpenTK.Toolkit.Init();
            InitializeComponent();
        }

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)

        {

            var flags = GraphicsContextFlags.Default;


            glControl = new GLControl(new GraphicsMode(32, 24), 2, 0, flags);

            glControl.MakeCurrent();
            glControl.Paint += GLControl_Paint;

            glControl.Dock = DockStyle.Fill;
            glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);

            (sender as WindowsFormsHost).Child = glControl;

        }


        private void GLControl_Paint(object sender, PaintEventArgs e)

        {

            GL.ClearColor( (float) 0.8, (float) 0.8, (float) 0.8, 1);

            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            

            //GL.Viewport(-100, -100, 10000, 10000);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 1000 / (float)1000, 1.0f, 64.0f);

            GL.MatrixMode(MatrixMode.Modelview);


            GL.MatrixMode(MatrixMode.Projection); //Load Perspective
            GL.LoadIdentity();
   
            //GL.LoadMatrix(ref projection);
            

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            cam.SetMatrix();

            //GL.MatrixMode(MatrixMode.Modelview);

            //GL.LoadMatrix(ref modelview);

            /*GL.Begin(BeginMode.Triangles);


            GL.Vertex3(-1.0f, -1.0f, 4.0f);

            GL.Vertex3(1.0f, -1.0f, 4.0f);

            GL.Vertex3(0.0f, 1.0f, 4.0f);

            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(-1.0f, -1.0f, 4.0f);

            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(1.0f, -1.0f, 4.0f);

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 4.0f);

            GL.End();
            */

            GL.Rotate(rotaX, Vector3d.UnitY);

            GL.Begin(BeginMode.Polygon);
            GL.Color3(0.0f, 1.0f, 0.0f);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100, 0, 0);
            GL.Vertex3(0, 25, 25);
            GL.End();

            
            GL.Rotate(rotaY, Vector3d.UnitY);

            GL.Begin(BeginMode.Polygon);
            GL.Color3(1.0f, 1.0f, 0.0f);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(200, 0, 0);
            GL.Vertex3(0, 25, 25);
            GL.End();
            

            GL.Begin(BeginMode.Lines);
            GL.Color3(1.0f, 1.0f, 0.0f);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 100, 0);
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Color3(0.0f, 1.0f, 1.0f);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3( 0, 0, 10);
            GL.End();

            //Rotating
            Vector2 xy_view_vector = new Vector2(0, 0);
            xy_view_vector.Normalize();
            Vector2 fold_vec = xy_view_vector.PerpendicularLeft; // this is the line over which you'd tilt the view forward towards the camera
            GL.Rotate((float)1, fold_vec.X, fold_vec.Y, 0.0f);// pitch (tilt forward)
            GL.Rotate((float)1, 0.0f, 0.0f, 1.0f);// yaw rotation - about z

            Console.WriteLine("Hello World!");
            glControl.SwapBuffers();

        }

        void SetupPerspective()
        {
            var aspectRatio = Width / (float)Height;
            Matrix4 Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 0.1f, 10);
            Matrix4 ModelView = Matrix4.Identity;
            // apply camera transform
            //Matrix4 Camera = Matrix4.LookAt(100, 20, 0, 0, 0, 0, 0, 1, 0);// 'Setup camera
            //Camera.ApplyCamera();
        }

        void ApplyCamera(ref Matrix4 modelView)
        {
            modelView = Matrix4.CreateRotationY(1)
                * Matrix4.CreateRotationX(1)
                * Matrix4.CreateRotationZ(1) 
                * Matrix4.CreateTranslation(0,0,0)
                * modelView;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)

        {

            glControl.Invalidate();

        }

        private void WindowsFormsHost_ChildChanged(object sender, ChildChangedEventArgs e)
        {

        }
        private void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //x = PointToClient(Cursor.Position).X * (z + 1);
            //y = (-PointToClient(Cursor.Position).Y + glControl.Height) * (z + 1);

            //SetupCursorXYZ();
            //Console.WriteLine("Hello!");
            //GL.Rotate( 10, Vector3d.UnitY);
            rotaX += 1;
            rotaY += 1;
            //cam.Zoom(2);
            glControl.Invalidate();
        }

    }
}