using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using RingRaceLibrary;

namespace RingRacesApp
{
    public partial class Form1 : Form
    {
        public partial class Form1 : Form
        {
            private void Form1_Load(object sender, EventArgs e)
            {
                // ����� ����� ���������� ���������������� ��� ��� �����,
                // ��������, ��������� ����������, ������������� ���������� � �.�.
            }
            // �������� ���������� OpenGL
            private GLControl glControl;
            // ��� �������
            private Car car1;
            private Car car2;

            // ����������� ����� ��� car1 (WASD)
            private bool moveForward1, moveBackward1, turnLeft1, turnRight1;
            // ����������� ����� ��� car2 (�������)
            private bool moveForward2, moveBackward2, turnLeft2, turnRight2;

            // ��� ���������� ������� ����� ������� (deltaTime)
            private DateTime lastFrameTime;

            public Form1()
            {
                InitializeComponent();
                Text = "2D Car Game with Two Cars";
                Width = 800;
                Height = 600;
                KeyPreview = true;  // ����� ����� �������� ������� ���������� ���� ��� ������� �������� ���������

                glControl = new GLControl(new GraphicsMode(32, 0, 0, 4))
                {
                    Dock = DockStyle.Fill
                };
                glControl.Load += GlControl_Load;
                glControl.Paint += GlControl_Paint;
                glControl.Resize += GlControl_Resize;

                // ��������� ���������� PreviewKeyDown ��� GLControl.
                glControl.PreviewKeyDown += GlControl_PreviewKeyDown; //���� �������� ���������
                Controls.Add(glControl);

                // �������������� ������ ������� � ������, ������� � �����
                car1 = new Car(new Vector2(glControl.Width / 2 - 100, glControl.Height / 2), Color.Red);
                // �������������� ������ ������� � ������, ������� � ������
                car2 = new Car(new Vector2(glControl.Width / 2 + 100, glControl.Height / 2), Color.Blue);

                lastFrameTime = DateTime.Now;

                // ������������� �� ������� ���������� ��� ��������� ���������� ������ ���������
                this.KeyDown += Form1_KeyDown;
                this.KeyUp += Form1_KeyUp;
            }

            private void GlControl_Load(object sender, EventArgs e)
            {
                GL.ClearColor(Color4.CornflowerBlue);
                SetupViewport();
            }

            private void GlControl_Resize(object sender, EventArgs e)
            {
                SetupViewport();
            }

            private void SetupViewport()
            {
                GL.Viewport(0, 0, glControl.Width, glControl.Height);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                // ��������������� �������� ��� 2D: (0,0) � ����� ������� ����
                GL.Ortho(0, glControl.Width, glControl.Height, 0, -1, 1);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
            }

            private void Form1_KeyDown(object sender, KeyEventArgs e)
            {
                // ���������� ��� ������ ������� (WASD)
                if (e.KeyCode == Keys.W)
                    moveForward1 = true;
                if (e.KeyCode == Keys.S)
                    moveBackward1 = true;
                if (e.KeyCode == Keys.A)
                    turnLeft1 = true;
                if (e.KeyCode == Keys.D)
                    turnRight1 = true;

                // ���������� ��� ������ ������� (�������)
                if (e.KeyCode == Keys.Up)
                    moveForward2 = true;
                if (e.KeyCode == Keys.Down)
                    moveBackward2 = true;
                if (e.KeyCode == Keys.Left)
                    turnLeft2 = true;
                if (e.KeyCode == Keys.Right)
                    turnRight2 = true;
            }

            private void Form1_KeyUp(object sender, KeyEventArgs e)
            {
                // ��� ������ ������� (WASD)
                if (e.KeyCode == Keys.W)
                    moveForward1 = false;
                if (e.KeyCode == Keys.S)
                    moveBackward1 = false;
                if (e.KeyCode == Keys.A)
                    turnLeft1 = false;
                if (e.KeyCode == Keys.D)
                    turnRight1 = false;

                // ��� ������ ������� (�������)
                if (e.KeyCode == Keys.Up)
                    moveForward2 = false;
                if (e.KeyCode == Keys.Down)
                    moveBackward2 = false;
                if (e.KeyCode == Keys.Left)
                    turnLeft2 = false;
                if (e.KeyCode == Keys.Right)
                    turnRight2 = false;
            }

            private void GlControl_Paint(object sender, PaintEventArgs e)
            {
                DateTime currentFrameTime = DateTime.Now;
                float deltaTime = (float)(currentFrameTime - lastFrameTime).TotalSeconds;
                lastFrameTime = currentFrameTime;

                // ��������� ��������� ����� ������� � ������ ������� ����� �������
                car1.Update(deltaTime, moveForward1, moveBackward1, turnLeft1, turnRight1);
                car2.Update(deltaTime, moveForward2, moveBackward2, turnLeft2, turnRight2);

                GL.Clear(ClearBufferMask.ColorBufferBit);

                // ��������� ����� �������
                car1.Draw();
                car2.Draw();

                glControl.SwapBuffers();
                glControl.Invalidate();  // �������������� ����������� ��� ������ �����
            }
            private void GlControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
            {
                // ���� ������ �������, ���������, ��� ��� ������� �������.
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                    e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {
                    e.IsInputKey = true;
                }
            }
        }
    }
}

