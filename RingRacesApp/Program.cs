using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.WinForms;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;

namespace WinFormsOpenTKCoreExample
{
    public class MainForm : Form
    {
        private GLControl glControl;
        private Label statusLabel;

        // �������������� ��� ��������� ���������, VBO, VAO � uniform-���������� ��������
        private int shaderProgram;
        private int vertexBuffer;
        private int vertexArray;
        private int uTranslationLocation;

        private readonly string vertexShaderSource =
            "#version 330 core\n" +
            "layout(location = 0) in vec2 aPosition;\n" +
            "layout(location = 1) in vec3 aColor;\n" +
            "out vec3 vertexColor;\n" +
            "uniform vec2 uTranslation;\n" +
            "void main()\n" +
            "{\n" +
            "    gl_Position = vec4(aPosition + uTranslation, 0.0, 1.0);\n" +
            "    vertexColor = aColor;\n" +
            "}\n";

        private readonly string fragmentShaderSource =
            "#version 330 core\n" +
            "in vec3 vertexColor;\n" +
            "out vec4 fragColor;\n" +
            "void main()\n" +
            "{\n" +
            "    fragColor = vec4(vertexColor, 1.0);\n" +
            "}\n";

        // ������ �������������� (��� ������������ � 6 ������)
        // ������ �������: 2 ���������� ������� � 3 ���������� �����
        private readonly float[] vertices = {
            // ������ �����������:
            -0.25f, -0.15f,   1f, 0f, 0f,
             0.25f, -0.15f,   0f, 1f, 0f,
             0.25f,  0.15f,   0f, 0f, 1f,
            // ������ �����������:
            -0.25f, -0.15f,   1f, 0f, 0f,
             0.25f,  0.15f,   0f, 0f, 1f,
            -0.25f,  0.15f,   1f, 1f, 0f
        };

        // ��������� (��������) �������������� � �������� (0,0)
        private Vector2 rectangleTranslation = Vector2.Zero;
        private const float moveSpeed = 0.02f;

        // ����� ������� ������
        private bool up, down, left, right;

        // ������ ���������� (WinForms Timer)
        private System.Windows.Forms.Timer timer;

        public MainForm()
        {
            Text = "����������� ������������� � ��������";
            Width = 800;
            Height = 600;

            // ��������� ���� �� ��� �������:
            // 1. ������ ��� ����������� ������ (������)
            // 2. ��������� ��� GLControl (�������� ���������� �������)

            // ������� ������ � ������������� ������� ��� �������
            Panel statusPanel = new Panel();
            statusPanel.Dock = DockStyle.Top;
            statusPanel.Height = 30;
            statusPanel.BackColor = System.Drawing.Color.Black;
            Controls.Add(statusPanel);

            // ������� Label ��� ������ ���������
            statusLabel = new Label();
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Font = new System.Drawing.Font("Consolas", 10);
            statusLabel.ForeColor = System.Drawing.Color.White;
            statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            statusPanel.Controls.Add(statusLabel);

            // ������� ������ ��� GLControl, ������� ������ ���������� ������� ����
            Panel glPanel = new Panel();
            glPanel.Dock = DockStyle.Fill;
            Controls.Add(glPanel);

            // ��� ��������� ������������ ������� �����
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            this.KeyUp += MainForm_KeyUp;

            // ������� GLControl ��� OpenGL 3.3 Core
            var glControlSettings = new GLControlSettings { APIVersion = new Version(3, 3) };
            glControl = new GLControl(glControlSettings)
            {
                Dock = DockStyle.Fill,
                TabStop = true  // ����� GLControl ��� �������� �����
            };
            glControl.Load += GlControl_Load;
            glControl.Paint += GlControl_Paint;
            glControl.Resize += GlControl_Resize;
            // ����� ����������� GLControl �� ������� ����������
            glControl.KeyDown += MainForm_KeyDown;
            glControl.KeyUp += MainForm_KeyUp;
            glPanel.Controls.Add(glControl);

            // ������ ���������� (�������� 60 FPS)
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 16; // ~16 ��
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { up = true; }
            if (e.KeyCode == Keys.S) { down = true; }
            if (e.KeyCode == Keys.A) { left = true; }
            if (e.KeyCode == Keys.D) { right = true; }
            // ����� �������� � ����� � ������� ��� ��������:
            Console.WriteLine($"KeyDown: {e.KeyCode}");
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { up = false; }
            if (e.KeyCode == Keys.S) { down = false; }
            if (e.KeyCode == Keys.A) { left = false; }
            if (e.KeyCode == Keys.D) { right = false; }
            Console.WriteLine($"KeyUp: {e.KeyCode}");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // ��������� ������� �������������� � ����������� �� ������� ������
            if (up) { rectangleTranslation.Y += moveSpeed; }
            if (down) { rectangleTranslation.Y -= moveSpeed; }
            if (left) { rectangleTranslation.X -= moveSpeed; }
            if (right) { rectangleTranslation.X += moveSpeed; }

            // ��������� ����� ����������� Label
            statusLabel.Text = $"Translation: ({rectangleTranslation.X:0.00}, {rectangleTranslation.Y:0.00})  " +
                               $"W: {up}  A: {left}  S: {down}  D: {right}";

            // ����������� ����������� GLControl
            glControl.Invalidate();
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            // ������������� OpenGL-����������
            GL.LoadBindings(new OpenTK.Windowing.GraphicsLibraryFramework.GLFWBindingsContext());
            Console.WriteLine("OpenGL version: " + GL.GetString(StringName.Version));

            GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

            SetupShaders();
            SetupBuffers();

            // �������� ����� GLControl, ����� �� �������� ������������ ���������
        }
        protected override void OnShown(EventArgs e) { base.OnShown(e); glControl.Focus(); } //���� ������ �����

        private void SetupShaders()
        {
            // ���������� ���������� �������
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vStatus);
            if (vStatus != (int)All.True)
                throw new Exception("������ ���������� ���������� �������: " + GL.GetShaderInfoLog(vertexShader));

            // ���������� ������������ �������
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fStatus);
            if (fStatus != (int)All.True)
                throw new Exception("������ ���������� ������������ �������: " + GL.GetShaderInfoLog(fragmentShader));

            // ��������� ������� � ���������
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus != (int)All.True)
                throw new Exception("������ ���������� ��������� ���������: " + GL.GetProgramInfoLog(shaderProgram));

            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // �������� ������� uniform-���������� ��� ��������
            uTranslationLocation = GL.GetUniformLocation(shaderProgram, "uTranslation");
        }

        private void SetupBuffers()
        {
            vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(vertexArray);

            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // ��������� �������� ������� (location = 0)
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // ��������� �������� ����� (location = 1)
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            glControl.Invalidate();
        }

        private void GlControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(shaderProgram);
            // �������� uniform-�������� � ������
            GL.Uniform2(uTranslationLocation, rectangleTranslation);

            GL.BindVertexArray(vertexArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            glControl.SwapBuffers();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (vertexBuffer != 0) GL.DeleteBuffer(vertexBuffer);
            if (vertexArray != 0) GL.DeleteVertexArray(vertexArray);
            if (shaderProgram != 0) GL.DeleteProgram(shaderProgram);
            base.OnFormClosed(e);
        }

        [STAThread]
        public static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
