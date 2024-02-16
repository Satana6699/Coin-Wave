using System;
using System.Windows;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace EditedTriangle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _numOfVertices = 0;
        private int _uColorLocation;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        //инициализация OpenGL и настройка начального состояния
        private void glControl_Load(object sender, EventArgs e) 
        {
            glControl.MakeCurrent();

            // Get a shader program ID
            int shaderProgram = InitShadersAndGetProgram();

            _numOfVertices = InitVertexBuffers();

            _uColorLocation = GL.GetUniformLocation(shaderProgram, "uColor");
            if (_uColorLocation < 0)
            {
                MessageBox.Show("Failed to get uColorLocation variable");
                return;
            }

            // Set a triangle color
            GL.Uniform3(_uColorLocation, 0.945f, 0.745f, 0.356f);

            // Set a color for clearing the glCotrol
            GL.ClearColor(new Color4(0.286f, 0.576f, 0.243f, 1f));
        }

        /* Эта функция вызывается каждый раз, когда элемент управления OpenGL нуждается в перерисовке,
         * например, когда окно было изменено или свернуто и развернуто2.
         * Здесь обычно происходит основная логика рендеринга.*/
        private void glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e) //рисовалка
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            // Очистите glControl с помощью заданного цвета
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (_numOfVertices != 0)
            {
                GL.DrawArrays(PrimitiveType.Triangles, 0, _numOfVertices);
            }

            // Поменяйте местами передний и задний буферы
            glControl.SwapBuffers();
        }

        /* Используется для инициализации буферов вершин в OpenGL. 
         * Буферы вершин - это объекты OpenGL, которые хранят данные вершин3.*/
        private int InitVertexBuffers()
        {
            float[] vertices = new float[]
            {
                0.0f, 0.5f,
                -0.5f, -0.5f,
                0.5f, -0.5f
            };
            int n = 3;

            int vbo;
            GL.GenBuffers(1, out vbo);

            // Получаем размер массива в байтах
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            int sizeInBytes = vertices.Length * sizeof(float);
            // Send the vertex array to a video card memory
            GL.BufferData(BufferTarget.ArrayBuffer, sizeInBytes, vertices, BufferUsageHint.StaticDraw);
            // Config the aPosition variable
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            return n;
        }

        /* Для инициализации шейдеров и получения ссылки на шейдерную программу. 
         * Шейдеры - это программы, которые исполняются на графическом процессоре и
         * используются для определения того, как рендерятся и отображаются вершины и пиксели*/
        private int InitShadersAndGetProgram()
        {
            string vertexShaderSource =
                "#version 140\n" +
                "in vec2 aPosition;" +
                "void main()" +
                "{" +
                "    gl_Position = vec4(aPosition, 1.0, 1.0);" +
                "}";

            string fragmentShaderSource =
                "#version 140\n" +
                "out vec4 fragColor;" +
                "uniform vec3 uColor;" +
                "void main()" +
                "{" +
                "    fragColor = vec4(uColor, 1.0);" +
                "}";

            // Vertex Shader
            int vShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vShader, vertexShaderSource);
            GL.CompileShader(vShader);
            // Check compilation
            int ok;
            GL.GetShader(vShader, ShaderParameter.CompileStatus, out ok);
            if (ok == 0)
            {
                string vShaderInfo = GL.GetShaderInfoLog(vShader);
                MessageBox.Show("Error in the vertex shader:\n" + vShaderInfo);
                return -1;
            }

            // Fragment Shader
            int fShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fShader, fragmentShaderSource);
            GL.CompileShader(fShader);
            GL.GetShader(fShader, ShaderParameter.CompileStatus, out ok);
            if (ok == 0)
            {
                string fShaderInfo = GL.GetShaderInfoLog(fShader);
                MessageBox.Show("Error in the fragment shader:\n" + fShaderInfo);
                return -1;
            }

            int program = GL.CreateProgram();
            GL.AttachShader(program, vShader);
            GL.AttachShader(program, fShader);
            GL.LinkProgram(program);
            GL.UseProgram(program);

            return program;
        }
        #region ButtonCollor

        /* для установки цвета фона в OpenGL.*/
        private void buttonSetBGColor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GL.ClearColor(dialog.Color);
                glControl.Invalidate();
            }

        }

        /*вызывается при нажатии кнопки для установки цвета трассировки*/
        private void buttonSetTRColor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                float r = dialog.Color.R / 255f;
                float g = dialog.Color.G / 255f;
                float b = dialog.Color.B / 255f;
                GL.Uniform3(_uColorLocation, r, g, b);
                glControl.Invalidate();
            }
        } 
        #endregion
    }
}