using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Runtime.InteropServices;


namespace Dimension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Global Variables

        /// <summary>
        /// Allows Shapes class to be accessed.
        /// </summary>
        Shapes Procedures = new Shapes();

        /// <summary>
        /// Allows QUats_Procedures class to be accessed.
        /// </summary>
        Quats QUat_Procedures = new Quats();

        #endregion

        #region Constructor

        /// <summary>
        /// Calls MainWindow.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Axis settlement

        /// <summary>
        /// Triggered when MainWindow is loaded.
        /// Sets the axis.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The Window that triggered the function.
        /// </param>
        /// 
        /// <param name="e">
        /// The action performed.
        /// </param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Stores the origin as a 3D point. (Used to avoid repetition)
            Point3D point3D = new Point3D(0, 0, 0);

            // Creates positive x-axis.
            CreateAxis(point3D, new Vector3D(25, 0, 0), Colors.Red);

            // Creates negative x-axis.
            CreateAxis(point3D, new Vector3D(-25, 0, 0), Colors.Red);

            // Creates positive y-axis.
            CreateAxis(point3D, new Vector3D(0, 25, 0), Colors.Blue);

            // Creates negative y-axis.
            CreateAxis(point3D, new Vector3D(0, -25, 0), Colors.Blue);

            // Creates positive z-axis.
            CreateAxis(point3D, new Vector3D(0, 0, 25), Colors.Green);

            // Creates negative z-axis.
            CreateAxis(point3D, new Vector3D(0, 0, -25), Colors.Green);

            // Starting point is set to center of the screen.
            startingPos = new Point(400, 200);

        }

        /// <summary>
        /// Creates axis
        /// </summary>
        /// 
        /// <param name="initialPoint">
        /// Point from which the axis is created from
        /// </param>
        /// 
        /// <param name="directionAndMagnitude">
        /// Direction of axis and its magnitude.
        /// </param>
        /// 
        /// <param name="axisColor">
        /// Color of the axis.
        /// </param>
        private void CreateAxis(Point3D initialPoint, Vector3D directionAndMagnitude, Color axisColor)
        {
            var geometryContainer = new ModelVisual3D();
            var shapeContainer = new GeometryModel3D();

            // Creates SmoothCylinder to create the axis.
            shapeContainer.Geometry = Procedures.AddSmoothCylinder(new MeshGeometry3D(), initialPoint,
                                                                directionAndMagnitude, 0.1, 20);

            // Sets the color of the axis.
            shapeContainer.Material = new DiffuseMaterial(new SolidColorBrush(axisColor));

            // Axis is output on 3D space.
            geometryContainer.Content = shapeContainer;
            GraphContainer.Children.Add(geometryContainer);
        }

        #endregion

        #region Mouse Visual Movement

        #region Enter ViewMode

        /// <summary>
        /// Tells whether the animations have occurred once already.
        /// </summary>
        bool AnimationsComplete = false;

        /// <summary>
        /// Triggers when click is performed in the grid.
        /// Applies a series of animations and changes cursor.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The grid that triggered the function.
        /// </param>
        /// 
        /// <param name="e">
        /// The action performed.
        /// </param>
        private void ViewPort3D_Container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the animation has occurred already...
            if (AnimationsComplete)
                // No more animations...
                return;

            #region Equation grids disappear

            // No more Equation grids can be added.
            Adder.IsEnabled = false;

            // Creates and double animation which modifes the Opacity of the grids.
            var EquationAnim = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };

            // Set to OpacityProperty.
            Storyboard.SetTargetProperty(EquationAnim, new PropertyPath(OpacityProperty));

            // Creates storyboard to contain animation.
            var storyBoard = new Storyboard();

            // DoubleAnimation is added to Storyboard.
            storyBoard.Children.Add(EquationAnim);

            // If Animation isn't currently on or isn't already completed...
            if (Equ_StP.Children[0].Opacity != 0)
            {
                // For each grid...
                foreach (Grid grid in Equ_StP.Children)
                {
                    // Set target to grid.
                    Storyboard.SetTarget(EquationAnim, grid);

                    // Animate the grid.
                    storyBoard.Begin();
                }
            }

            #endregion

            #region StackPanel Green to White

            // Creates ColorAnimation to animate StackPanel's background.
            var backGroundAnim = new ColorAnimation
            {
                From = ((SolidColorBrush)Equ_StP.Background).Color,
                To = Brushes.White.Color,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                BeginTime = TimeSpan.FromSeconds(0.3)
            };

            // Sets the property the ColorAnimation needs to animate.
            Storyboard.SetTargetProperty(backGroundAnim, new PropertyPath("(StackPanel.Background).(SolidColorBrush.Color)"));

            // Sets the target to animate to be the StackPanel.
            Storyboard.SetTarget(backGroundAnim, Equ_StP);

            #endregion

            #region StackPanelSlideLeft

            // Creates animation that shrinks a columndef
            var SlideLeftAnimation = new DoubleAnimation
            {
                From = 525,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                BeginTime = TimeSpan.FromSeconds(0.8)
            };

            // Allows instructions to be performed when animation is completed.
            SlideLeftAnimation.Completed += SlideLeftAnimation_Completed;

            // Sets the animation to the target and sets the property to animate.
            Storyboard.SetTarget(SlideLeftAnimation, StP_Width);
            Storyboard.SetTargetProperty(SlideLeftAnimation, new PropertyPath("(ColumnDefinition.MaxWidth)"));

            #endregion

            #region PushUp

            // Create Animation to shrink upper part of the grid.
            var SlideUp = new DoubleAnimation
            {
                From = 50,
                To = 0,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                BeginTime = TimeSpan.FromSeconds(0.8)
            };

            // Sets target and property to animate.
            Storyboard.SetTarget(SlideUp, rowDef);
            Storyboard.SetTargetProperty(SlideUp, new PropertyPath("(RowDefinition.MaxHeight)"));

            #endregion

            #region Storyboarding

            // Creates Storyboard to contain all the previosuly made animations.
            var multipleAnimationsContainer = new Storyboard();

            // Adds the animations to the storyboard.
            multipleAnimationsContainer.Children.Add(backGroundAnim);
            multipleAnimationsContainer.Children.Add(SlideLeftAnimation);
            multipleAnimationsContainer.Children.Add(SlideUp);

            // Animates the animations contained in the Storyboard.
            multipleAnimationsContainer.Begin();

            #endregion

            // Makes cursor into a cross.
            this.Cursor = Cursors.Cross;

            // Set bool to true.
            AnimationsComplete = true;

        }

        #endregion

        #region Exit ViewMode

        #region Enter Animations Aftermath

        /// <summary>
        /// Triggeres when last animation is completed.
        /// Removes StackPanel to allow free movement of cursors.
        /// </summary>
        /// 
        /// <param name="sender">
        /// ANimation which triggered the function.
        /// </param>
        /// 
        /// <param name="e">
        /// The action performed.
        /// </param>
        private void SlideLeftAnimation_Completed(object sender, EventArgs e)
        {
            // Removes StackPanel.
            gridSubSplitter.Children.Remove(Equ_StP);

            #region User Helper placement

            // Creates TextBlock that contains text.
            var escTextBlock = new TextBlock
            {
                Text = "ESC",
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 30,
                Foreground = Brushes.White
            };

            // Creates border to add decorations
            var decorativeBorder = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10)
            };

            //Border contains TextBlock.
            decorativeBorder.Child = escTextBlock;

            // Creates border to contains the other controls.
            var roundCorners = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#152238")),
                CornerRadius = new CornerRadius(10),
                BorderThickness = new Thickness(0),
                Width = 200,
                Height = 100,
                Margin = new Thickness(15, 15, ViewPort3D_Container.ActualWidth - 215, ViewPort3D_Container.ActualHeight - 140)
            };

            roundCorners.Child = decorativeBorder;

            // Sets helper into right grid section.
            Grid.SetColumn(roundCorners, 1);

            // Adds helper to Grid.
            ViewPort3D_Container.Children.Add(roundCorners);

            #endregion

            #region Helper Animations

            // Creates animation that enlarges the helper's width and rewids the animation.
            var widthHelperAnimation = new DoubleAnimation
            {
                From = 200,
                To = 150,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Sets target of the aniamtion
            Storyboard.SetTarget(widthHelperAnimation, roundCorners);

            // Sets property ot animate.
            Storyboard.SetTargetProperty(widthHelperAnimation, new PropertyPath(WidthProperty));

            // Creates animation that enlarges the helper's height and rewids the animation.
            var heightHelperAnimation = new DoubleAnimation
            {
                From = 100,
                To = 75,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Sets target of the aniamtion
            Storyboard.SetTarget(heightHelperAnimation, roundCorners);

            // Sets property ot animate.
            Storyboard.SetTargetProperty(heightHelperAnimation, new PropertyPath(HeightProperty));

            // Creates animation that enlarges the text's size and rewids the animation.
            var textBlockAnimation = new DoubleAnimation
            {
                From = 30,
                To = 20,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            // Sets target of the aniamtion
            Storyboard.SetTarget(textBlockAnimation, escTextBlock);

            // Sets property ot animate.
            Storyboard.SetTargetProperty(textBlockAnimation, new PropertyPath(FontSizeProperty));

            // Creates storyboard to contain all the previously created animations.
            var storyboard = new Storyboard();

            // Adds the animations to the storyboard.
            storyboard.Children.Add(widthHelperAnimation);
            storyboard.Children.Add(heightHelperAnimation);
            storyboard.Children.Add(textBlockAnimation);

            // Starts the animations. 
            storyboard.Begin();

            #endregion

            // Disallows the function to run again after it has been executed once.
            ViewPort3D_Container.PreviewKeyDown += ViewPort3D_Container_PreviewKeyDown;

            // Sets focus on grid.
            ViewPort3D_Container.Focus();
        }

        #endregion

        #region Escape

        private void ViewPort3D_Container_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {

                // if Escape key is pressed.
                case Key.Escape:

                    #region PushDown

                    // Superior panel slides down through this animation.
                    var slideDown = new DoubleAnimation
                    {
                        From = 0,
                        To = 50,
                        Duration = new Duration(TimeSpan.FromSeconds(0.3))
                    };

                    // Sets the animation to the target and sets the property to animate.
                    Storyboard.SetTarget(slideDown, rowDef);
                    Storyboard.SetTargetProperty(slideDown, new PropertyPath("(RowDefinition.MaxHeight)"));

                    #endregion

                    #region Slide to the right

                    // Adds panel to the grid.
                    gridSubSplitter.Children.Add(Equ_StP);

                    // Creates animation for slide to the right.
                    var slideRightAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 525,
                        Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                        BeginTime = TimeSpan.FromSeconds(0.3)
                    };

                    // Sets the animation to the target and sets the property to animate.
                    Storyboard.SetTarget(slideRightAnimation, StP_Width);
                    Storyboard.SetTargetProperty(slideRightAnimation, new PropertyPath("(ColumnDefinition.MaxWidth)"));

                    #endregion

                    #region Background change

                    // Creates ColorAnimation to animate StackPanel's background.
                    var backGroundAnim = new ColorAnimation
                    {
                        From = ((SolidColorBrush)Equ_StP.Background).Color,
                        To = (Color)ColorConverter.ConvertFromString("#699b2c"),
                        Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                        BeginTime = TimeSpan.FromSeconds(0.6)
                    };

                    // Sets the property the ColorAnimation needs to animate.
                    Storyboard.SetTargetProperty(backGroundAnim, new PropertyPath("(StackPanel.Background).(SolidColorBrush.Color)"));

                    // Sets the target to animate to be the StackPanel.
                    Storyboard.SetTarget(backGroundAnim, Equ_StP);

                    #endregion

                    #region 3 Anim-Storyboards

                    // Storyboard that contains animations.
                    var multipleAnimationsContainer = new Storyboard();

                    // Animations added to the storyboard.
                    multipleAnimationsContainer.Children.Add(slideDown);
                    multipleAnimationsContainer.Children.Add(slideRightAnimation);
                    multipleAnimationsContainer.Children.Add(backGroundAnim);

                    // Animations start.
                    multipleAnimationsContainer.Begin();

                    #endregion

                    #region Euqations appear

                    // Creates and double animation which modifes the Opacity of the grids.
                    var EquationAnim = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                        BeginTime = TimeSpan.FromSeconds(0.11)
                    };


                    // Set to OpacityProperty.
                    Storyboard.SetTargetProperty(EquationAnim, new PropertyPath(OpacityProperty));

                    // Creates storyboard to contain animation.
                    var singleAnimationContainer = new Storyboard();

                    // DoubleAnimation is added to Storyboard.
                    singleAnimationContainer.Children.Add(EquationAnim);

                    // For each grid...
                    foreach (Grid grid in Equ_StP.Children)
                    {
                        // Set target to grid.
                        Storyboard.SetTarget(EquationAnim, grid);

                        // Animate the grid.
                        singleAnimationContainer.Begin();
                    }

                    #endregion

                    // Helper is removed.
                    ViewPort3D_Container.Children.Remove(ViewPort3D_Container.Children[1]);

                    // Adder button is enabled.
                    Adder.IsEnabled = true;

                    // Animations to tenter view mode are now allowed again.
                    AnimationsComplete = false;

                    // This whole function can no longer be run after being executed once.
                    ViewPort3D_Container.PreviewKeyDown -= ViewPort3D_Container_PreviewKeyDown;

                    // Sets cursors back to normal.
                    this.Cursor = Cursors.Arrow;

                    break;
            }

        }

        #endregion

        #endregion

        #region MouseMove

        /// <summary>
        /// Sets cursor to specified coordinates.
        /// </summary>
        /// 
        /// <param name="x">
        /// X-coordinate.
        /// </param>
        /// 
        /// <param name="y">
        /// Y-coordinate.
        /// </param>
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// Stores the center of the screen as a 2D Point.
        /// </summary>
        Point startingPos;

        /// <summary>
        /// Triggered when mouse moves within the grid.
        /// Rotates the direction the camera is currently looking at while mouse moves.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The grid in which the mouse is on.
        /// </param>
        /// 
        /// <param name="e">
        /// The action performed.
        /// </param>
        private void ViewPort3D_Container_MouseMove(object sender, MouseEventArgs e)
        {
            // Gets a 3D vector which is perpendicular to the current LookDirection of the camera the Y-axis.
            var cameraHorizontalAxis = Vector3D.CrossProduct(Persp_Cam.LookDirection, new Vector3D(0, 1, 0));

            // Gets new position of the cursor on the grid.
            var endPos = Mouse.GetPosition((Grid)sender);

            // Cursor gets back to the center if it touches the screen borders.
            if ((endPos.X >= 0 && endPos.X <= 1) || endPos.Y == 0 || endPos.X == this.ActualWidth - 1 || endPos.Y == this.ActualHeight - 1)
                SetCursorPos((int)gridSubSplitter.ActualWidth / 2, (int)gridSubSplitter.ActualHeight / 2);

            // Gets 2D vector direction from the starting point to thenew one.
            var relativeVect = endPos - startingPos;

            // Gets the 2D perpendicular vector of the previously calculated one.
            var perpendicularVect = new Vector(relativeVect.Y, -relativeVect.X);

            // Gets the angle between the perpendicular vector and the x-axis.
            double AngleToRotateAxis = Vector.AngleBetween(new Vector(-1, 0), perpendicularVect);

            /* An Axis of rotation (stores as 3D vector) is created by rotating the cameraHorizontalAxis by AngleToRotateAxis
             * in the current direction of the camera. */
            var AxisOfRotation = QUat_Procedures.RotateVector3D(cameraHorizontalAxis, AngleToRotateAxis, Persp_Cam.LookDirection);

            // Rotates the camera direction based on the previously calculated Axis and the the fixed angle value of 0.35.
            Persp_Cam.LookDirection = QUat_Procedures.RotateVector3D(Persp_Cam.LookDirection, 0.35, AxisOfRotation);

            // StartingPos is set to endPos.
            startingPos = endPos;

            /* NOTE: This subroutine can be extremely hard to understand as it covers maths topics from University
             * and takes advantage of complex concepts. */
        }

        #endregion

        #endregion

        #region Camera Position

        /// <summary>
        /// Moves Camera position.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The key pressed.
        /// </param>
        /// 
        /// <param name="e">
        /// Action performed.
        /// </param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Stores the direction of the camera.
            var cameraDirection = Persp_Cam.LookDirection;

            // Stores position of the camera.
            var Camera_Pos = Persp_Cam.Position;

            switch (e.Key)
            {
                // if D or right arrow are pressed...
                case Key.D:
                case Key.Right:

                    // 3D vector perpendicular to Y-axis and camera direction is calculated.
                    var perpendicularVec = Vector3D.CrossProduct(cameraDirection, new Vector3D(0, 1, 0));

                    // Unit 3D vector of the perpendicular 3D vector is calculated.
                    perpendicularVec.Normalize();

                    // Camera position is moved through unit 3D vector.
                    Camera_Pos += perpendicularVec;

                    break;

                // if A or left arrow are pressed.
                case Key.A:
                case Key.Left:

                    // 3D vector perpendicular to negative Y-axis and camera direction is calculated.
                    var perpendicularVec2 = Vector3D.CrossProduct(cameraDirection, new Vector3D(0, -1, 0));

                    // Unit 3D vector of the perpendicular 3D vector is calculated.
                    perpendicularVec2.Normalize();

                    // Camera position is moved through unit 3D vector.
                    Camera_Pos += perpendicularVec2;

                    break;

                // if W or Up arrow are pressed...
                case Key.W:
                case Key.Up:

                    // Unit 3D vector of the perpendicular 3D vector is calculated.
                    cameraDirection.Normalize();

                    // Camera position is moved through unit 3D vector.
                    Camera_Pos += cameraDirection;

                    break;

                // if S or down arrow are pressed...
                case Key.S:
                case Key.Down:

                    // The opposite 3D vector of the camera direction is calculated.
                    cameraDirection.X *= -1;
                    cameraDirection.Y *= -1;
                    cameraDirection.Z *= -1;

                    // Unit 3D vector of the opposite 3D vector is calculated.
                    cameraDirection.Normalize();

                    // Camera position is moved through unit 3D vector.
                    Camera_Pos += cameraDirection;
                    break;

                case Key.Space:

                    // 3D vector perpendicular to negative Z-axis and camera direction is calculated.
                    var perpendicularVec3 = Vector3D.CrossProduct(cameraDirection, new Vector3D(-1, 0, 0));

                    // Unit 3D vector of the opposite 3D vector is calculated.
                    perpendicularVec3.Normalize();

                    // Camera position is moved through unit 3D vector.
                    Camera_Pos += perpendicularVec3;
                    break;

                case Key.LeftShift:
                case Key.RightShift:

                    // 3D vector perpendicular to Z-axis and camera direction is calculated.
                    var perpendicularVec4 = Vector3D.CrossProduct(cameraDirection, new Vector3D(1, 0, 0));

                    // Unit 3D vector of the opposite 3D vector is calculated.
                    perpendicularVec4.Normalize();

                    // Camera position is moved through unit 3D vector.
                    Camera_Pos += perpendicularVec4;

                    break;
            }

            // New camera position is set.
            Persp_Cam.Position = Camera_Pos;

        }

        #endregion

        #region 3DShaper

        #region Creation of shapes

        /// <summary>
        /// Generates 3D shapes.
        /// </summary>
        /// 
        /// <param name="sender">
        /// The button that triggered the function.
        /// </param>
        /// 
        /// <param name="e">
        /// What type of event has performed.
        /// </param>
        private void btn_Submit_Click(object sender, RoutedEventArgs e)
        {
            // Gets grid in which the control is at.
            var grid = (Grid)((ToolBar)((Button)sender).Parent).Parent;

            // Gets the TextBlock where a line (or another object) is specified.
            var Labeller = (TextBlock)grid.Children[0];

            // Gets TextBlock where the equation is shown.
            var Equation = (TextBlock)grid.Children[1];

            // Queue is made to store coordinates.
            var Coordinates = new Queue<string>();

            // Contains shape to be created.
            var Mesh_Container = new GeometryModel3D();

            // Try code...
            try
            {
                // for each object within the TextBlock...
                foreach (Inline inline in Equation.Inlines)
                {

                    // if the object is the container of the TextBoxes the code is looking for...
                    if (inline is InlineUIContainer)
                    {

                        // The TextBox is found and stored.
                        var InnerTB = (TextBox)((InlineUIContainer)inline).Child;

                        // String stores the CHECKED and MODIFIED text of the TextBox.
                        var tempString = CheckCoordinate(InnerTB.Text);

                        // New string is re-assigned to TextBox and assigned to Queue.
                        InnerTB.Text = tempString;
                        Coordinates.Enqueue(tempString);
                    }

                }

            }
            // and if the code throws error...
            catch (InvalidCastException)
            {
                // Message is displayed to the user.
                MessageBox.Show("Fill in all the TextBoxes");
            }

            #region Line

            // If "Line" is selected...
            if (Labeller.Text == "Line:")
            {

                // Coordinates are popped off the queue and assigned to the Point3D and Vector3D.
                var point3d = new Point3D(double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()));
                var vector3d = new Vector3D(double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()));

                // Enlarged version of the Point3D and Vector3D are made.
                var enlargedPoint3D = ExpandPoint3D(point3d, vector3d);
                var enlargedVector3D = ExpandVector3D(point3d, vector3d);

                // The shape is created based on the enlarged point and vector.
                Mesh_Container.Geometry = Procedures.AddSmoothCylinder(new MeshGeometry3D(), enlargedPoint3D, enlargedVector3D, 0.1, 20);
                Mesh_Container.Material = new DiffuseMaterial(Brushes.Blue);

            }

            #endregion

            #region Sphere

            // Else if "Sphere" is selected...
            else if (Labeller.Text == "Sphere:")
            {

                // radius is stored.
                var tempString2 = Coordinates.Dequeue();

                // Checkes whether radius is negative and removes it, if it is.
                if (tempString2.Contains('-'))
                    tempString2 = tempString2.TrimStart('-');

                // Stores radius as double.
                var radius = double.Parse(tempString2);

                // Stores center as 3D point.
                Point3D center = new Point3D(double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()));

                // The shape is created based on the center and radius.
                Mesh_Container.Geometry = Procedures.AddSphere(new MeshGeometry3D(), center, radius, 30, 30);
                Mesh_Container.Material = new DiffuseMaterial(Brushes.Red);

            }

            #endregion

            #region Point

            // Else if "Point" is selected...
            else if (Labeller.Text == "Point:")
            {
                // Coordinates are popped off the queue and assigned to center.
                var center = new Point3D(double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()));

                // The point is created based on center.
                Mesh_Container.Geometry = Procedures.AddSphere(new MeshGeometry3D(), center, 0.1, 30, 30);
                Mesh_Container.Material = new DiffuseMaterial(Brushes.Yellow);

            }

            #endregion

            #region Cube

            else if (Labeller.Text == "Cube:")
            {
                // Coordinates are popped off the queue and assigned to center.
                var center = new Point3D(double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()));

                // Length of cube specified is popped off the queue.
                var length = float.Parse(Coordinates.Dequeue());

                // Cube is created.
                Mesh_Container.Geometry = Procedures.AddCube(center, length);
                Mesh_Container.Material = new DiffuseMaterial(Brushes.Aqua);
            }

            #endregion

            #region Pyramid

            else if (Labeller.Text == "Pyramid:")
            {
                // Coordinates are popped off the queue and assigned to center.
                var center = new Point3D(double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()), double.Parse(Coordinates.Dequeue()));

                // Length of pyramid specified is popped off the queue.
                var length = float.Parse(Coordinates.Dequeue());

                // Pyramid is created.
                Mesh_Container.Geometry = Procedures.AddPyramid(center, length);
                Mesh_Container.Material = new DiffuseMaterial(Brushes.Orange);
            }

            #endregion

            // Contains geometryModel.
            var Geometry_Container = new ModelVisual3D();

            // Assigns geometry model to container.
            Geometry_Container.Content = Mesh_Container;

            // Shape added to 3D space.
            GraphContainer.Children.Add(Geometry_Container);
        }

        #endregion

        #region Are the coordinates valid?

        /// <summary>
        /// Checks if the coordinates input as string are valid.
        /// </summary>
        /// 
        /// <param name="coordinate">
        /// The input coordinate as a string.
        /// </param>
        /// 
        /// <returns>
        /// - Valid coordinate as string.
        /// - Or throws Exception.
        /// </returns>
        private string CheckCoordinate(string coordinate)
        {
            // If user inputs a coordinate that ends with a dot...
            if (coordinate != "" && coordinate[coordinate.Length - 1] == '.')

                // dot is removed.
                return coordinate.Remove(coordinate.IndexOf('.'), 1);

            // if no input...
            else if (coordinate == string.Empty)
                // Exception thrown.
                throw new InvalidCastException();

            // If input valid...
            else

                // returned as valid.
                return coordinate;
        }

        #endregion

        #region Maths for line

        /// <summary>
        /// Enlarges a 3D point.
        /// </summary>
        /// 
        /// <param name="BasePoint">
        /// 3D point to enlarge.
        /// </param>
        /// 
        /// <param name="ParallelVector">
        /// 3D vector used to enlarge BasePoint.
        /// </param>
        /// 
        /// <returns>
        /// Enlarged 3D point.
        /// </returns>
        private Point3D ExpandPoint3D(Point3D BasePoint, Vector3D ParallelVector)
        {
            // Constant is made.
            double t = (25 - BasePoint.X) / ParallelVector.X;

            // Using algebra and constat to find the other two new enlarged coordinates.
            return new Point3D(25, BasePoint.Y + t * ParallelVector.Y, BasePoint.Z + t * ParallelVector.Z);
        }

        /// <summary>
        /// Enlarges a 3D vector.
        /// </summary>
        /// 
        /// <param name="BasePoint">
        /// 3D point used to enlarge ParallelVector.
        /// </param>
        /// 
        /// <param name="ParallelVector">
        /// 3D vector that needs to be enlarged.
        /// </param>
        /// 
        /// <returns>
        /// Enlarged 3D vector.
        /// </returns>
        private Vector3D ExpandVector3D(Point3D BasePoint, Vector3D ParallelVector)
        {
            // Constant is made.
            double t = (-25 - BasePoint.X) / ParallelVector.X;

            // New point is made with ParallelVector,
            Point3D pointB = new Point3D(-25, BasePoint.Y + t * ParallelVector.Y, BasePoint.Z + t * ParallelVector.Z);

            //Enlarged 3D vector is made and returned.
            return pointB - BasePoint;
        }

        #endregion

        #endregion

        #region MenuItems trigger functions

        /// <summary>
        /// Switch between equations and sphere and more...
        /// </summary>
        /// 
        /// <param name="sender">
        /// MenuItem that triggers the fucntion,
        /// </param>
        /// 
        /// <param name="e">
        /// The event performed.
        /// </param>
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            // gets the tirggering MenuItem.
            var menuItem = (MenuItem)sender;

            // Gets Menu to access the Grid parent.
            var parentMenu = (Menu)((MenuItem)menuItem.Parent).Parent;

            // Grid is accessed
            var parentGrid = (Grid)((ToolBar)parentMenu.Parent).Parent;

            // TextBlock (where object is specified) is accessed.
            var Labeller = (TextBlock)parentGrid.Children[0];

            // TextBlock (equation) is accessed.
            var Equation = (TextBlock)parentGrid.Children[1];

            #region SphereOption

            // If it's currently an equation for lines but the option for sphere is clicked...
            if (menuItem.Header.ToString() == "Sphere")
            {
                // Removes current elements in the TextBlock.
                foreach (Inline inline in Equation.Inlines.ToList())
                    Equation.Inlines.Remove(Equation.Inlines.FirstInline);

                // Adds writings and TextBoxes
                Equation.Inlines.Add(new Run("Radius: "));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run("; Center: ("));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));
                Equation.Inlines.Add(new Run(");"));

                // Option selected turns into Sphere.
                Labeller.Text = "Sphere:";
            }

            #endregion

            #region LineOption

            // If it's currently an equation for spheres but the option for line is clicked...
            else if (menuItem.Header.ToString() == "Line")
            {
                // Removes current elements in the TextBlock.
                foreach (Inline inline in Equation.Inlines.ToList())
                    Equation.Inlines.Remove(Equation.Inlines.FirstInline);

                // Adds writings and TextBoxes
                Equation.Inlines.Add(new Run("r = ("));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(") + t ("));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));
                Equation.Inlines.Add(new Run(")"));

                // Option selected turns into Line.
                Labeller.Text = "Line:";
            }

            #endregion

            #region PointOption

            // If option is point...
            else if (menuItem.Header.ToString() == "Point")
            {
                // Removes current elements in the TextBlock.
                foreach (Inline inline in Equation.Inlines.ToList())
                    Equation.Inlines.Remove(Equation.Inlines.FirstInline);

                // Adds writings and TextBoxes
                Equation.Inlines.Add(new Run("Point = ("));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(")"));

                // Option selected turns into Point.
                Labeller.Text = "Point:";
            }

            #endregion

            #region CubeOption

            // If option is point...
            else if (menuItem.Header.ToString() == "Cube")
            {
                // Removes current elements in the TextBlock.
                foreach (Inline inline in Equation.Inlines.ToList())
                    Equation.Inlines.Remove(Equation.Inlines.FirstInline);

                // Adds writings and TextBoxes
                Equation.Inlines.Add(new Run("Centre = ("));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(")  Length:"));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                // Option selected turns into Cube.
                Labeller.Text = "Cube:";
            }

            #endregion

            #region PyramidOption

            // If option is point...
            else if (menuItem.Header.ToString() == "Pyramid")
            {
                // Removes current elements in the TextBlock.
                foreach (Inline inline in Equation.Inlines.ToList())
                    Equation.Inlines.Remove(Equation.Inlines.FirstInline);

                // Adds writings and TextBoxes
                Equation.Inlines.Add(new Run("Centre = ("));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(","));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                Equation.Inlines.Add(new Run(")  Length:"));
                Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

                // Option selected turns into Pyramid.
                Labeller.Text = "Pyramid:";
            }

            #endregion

        }

        #region TextBox style setter

        /// <summary>
        /// Makes a new UiInlineContainer with desired attributes.
        /// </summary>
        /// 
        /// <param name="BaseTB">
        /// TextBox to be modified.
        /// </param>
        /// 
        /// <param name="parentGrid">
        /// Grid from where the Style needs to be accessed from.
        /// </param>
        /// 
        /// <returns>
        /// new Modified UiInlineContainer.
        /// </returns>
        private InlineUIContainer InlineUIContainerFormatter(TextBox BaseTB, Grid parentGrid)
        {
            // Style  acccessed and assigned to textBox style.
            BaseTB.Style = parentGrid.FindResource("TB_KeyStyle") as Style;

            // New InlineUIContainer is created and modified.
            var newInlineContainer = new InlineUIContainer(BaseTB);
            newInlineContainer.BaselineAlignment = BaselineAlignment.Center;

            // Modified textBox is returned.
            return newInlineContainer;
        }

        #endregion

        #endregion

        #region Adder

        /// <summary>
        /// Adds new Equation section.
        /// </summary>
        /// 
        /// <param name="sender">
        /// Button that triggers the function.
        /// </param>
        /// 
        /// <param name="e">
        /// The action performed.
        /// </param>
        private void Adder_Click(object sender, RoutedEventArgs e)
        {
            // New Grid is made and defined.
            var newGrid = new Grid
            {
                Height = 100,
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#b8cf69")
            };

            // RowDefinitions are made to place controls in symmetrically.
            var rowDef1 = new RowDefinition();
            rowDef1.Height = new GridLength(30);

            var rowDef2 = new RowDefinition();
            rowDef2.Height = new GridLength(50);

            // RowDefinitions are then set on the Grid.
            newGrid.RowDefinitions.Add(rowDef1);
            newGrid.RowDefinitions.Add(rowDef2);
            newGrid.RowDefinitions.Add(new RowDefinition());

            // New textBlock is made and defined (specifies whthere it is a line or somethign else).
            var Labeller = new TextBlock
            {
                TextAlignment = TextAlignment.Left,
                Width = 500,
                Height = 30,
                Padding = new Thickness(5, 5, 5, 5),
                FontSize = 20,
                Foreground = Brushes.Black,
                FontFamily = new FontFamily("Impacted 2.0"),
                Background = Brushes.Transparent,
                Text = "Line:",
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
            };

            // Sets the TextBlock in first row on Grid.
            Grid.SetRow(Labeller, 0);

            // New TextBlock for equation input is made and defined.
            var Equation = new TextBlock
            {
                Height = 50,
                Width = 525,
                TextAlignment = TextAlignment.Center,
                Padding = new Thickness(10),
                FontSize = 25,
                FontFamily = new FontFamily("Impacted 2.0"),
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
            };

            // TextBlock is placed in row 1 on Grid.
            Grid.SetRow(Equation, 1);

            // Adds writings and TextBoxes.
            Equation.Inlines.Add(new Run("r = ("));
            Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

            Equation.Inlines.Add(new Run(","));
            Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

            Equation.Inlines.Add(new Run(","));
            Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

            Equation.Inlines.Add(new Run(") + t ("));
            Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

            Equation.Inlines.Add(new Run(","));
            Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));

            Equation.Inlines.Add(new Run(","));
            Equation.Inlines.Add(InlineUIContainerFormatter(new TextBox(), (Grid)Equ_StP.Children[0]));
            Equation.Inlines.Add(new Run(")"));

            // New ToolBar is made to contain controls.
            var controlsAligner = new ToolBar
            {
                Background = Brushes.Transparent,
                Width = 300,
            };

            // Set on Row 2 on Grid.
            Grid.SetRow(controlsAligner, 2);

            // Creates MenuItem for Line selection.
            var lineOption = new MenuItem
            {
                Header = "Line"
            };

            // Creates MenuItem for Sphere selection.
            var SphereOption = new MenuItem
            {
                Header = "Sphere"
            };

            // Creates MenuItem for Point selection.
            var PointOption = new MenuItem
            {
                Header = "Point"
            };

            // Creates MenuItem for Cube selection.
            var CubeOption = new MenuItem
            {
                Header = "Cube"
            };

            // Creates MenuItem for Pyramid selection.
            var PyramidOption = new MenuItem
            {
                Header = "Pyramid"
            };

            // Contains all the options made above.
            var anotherMenuItem = new MenuItem
            {
                Header = "Menu",
                FontSize = 7
            };

            // Options are contained in a MenuItem.
            anotherMenuItem.Items.Add(lineOption);
            anotherMenuItem.Items.Add(SphereOption);
            anotherMenuItem.Items.Add(PointOption);
            anotherMenuItem.Items.Add(CubeOption);
            anotherMenuItem.Items.Add(PyramidOption);

            // Adds Event click handler to each MenuItem in Menu.
            foreach (MenuItem menuItem in anotherMenuItem.Items)
                menuItem.Click += Option_Click;

            // Container then is added inside a Menu.
            var justAMenu = new Menu();
            justAMenu.Items.Add(anotherMenuItem);

            // A button is made and defined.
            var newButton = new Button
            {
                Background = Brushes.Wheat,
                Height = 20,
                Width = 240,
                Content = "Draw",
                FontSize = 7
            };

            // Adds event handler for click action.
            newButton.Click += btn_Submit_Click;

            // Button and Menu are added onto ToolBar.
            controlsAligner.Items.Add(justAMenu);
            controlsAligner.Items.Add(newButton);

            // Everything is then added on the Grid.
            newGrid.Children.Add(Labeller);
            newGrid.Children.Add(Equation);
            newGrid.Children.Add(controlsAligner);

            // Grid is added on the StackPanel.
            Equ_StP.Children.Add(newGrid);

        }

        #endregion

        #region TextChecks
        /// <summary>
        /// Checks that nothing other than numbers and dots can be input.
        /// </summary>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // RegularExpression created represents digits and dots.
            var NoLetters = new Regex("^[0-9.-]$");

            // If anything other than digits and dots is input
            // it will not be displayed into the TextBox.
            e.Handled = !NoLetters.IsMatch(e.Text);

            // Stores Text of TextBox.
            var InputString = ((TextBox)sender).Text;

            // If textBox contains a dot already and another dot is being input...
            if ((InputString.Contains('.') && e.Text.Contains('.')) || (InputString != string.Empty && e.Text.Contains('-')))

                //Extra dot is not inserted.
                e.Handled = true;

            // Restrict the user to input only 3 digits before the decimal point.
            if (((!InputString.Contains('.') && InputString.Contains('-') && InputString.Length == 4) ||
                (!InputString.Contains('.') && InputString.Length == 3 && !InputString.Contains('-')))
                && new Regex("[0-9]").IsMatch(e.Text))
                e.Handled = true;

            // Restrict the user to input a number up to 2 decimal points.
            if (InputString.Contains('.'))
                if (InputString.Remove(0, InputString.IndexOf('.') + 1).Length > 1)
                    e.Handled = true;


        }

        /// <summary>
        /// - Checks whether the first character input is a dot.
        ///   If true, it won't be displayed.
        /// - Enlarges TextBox as text overflows TextBox's width.
        /// - Disallows dots to be input after minus sign.
        /// - Disallows white space.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var TriggerTB = (TextBox)sender;

            // If first character input = '.', return empty string.
            if (TriggerTB.Text != "" && TriggerTB.Text[0] == '.')
            {
                TriggerTB.Text = "";
            }

            // If a dot in input after a minus sign, it won't be displayed.
            if (TriggerTB.Text.Contains("-."))
            {
                TriggerTB.Text = TriggerTB.Text.Remove(TriggerTB.Text.Length - 1, 1);
                TriggerTB.SelectionStart = TriggerTB.Text.Length;
            }

            // Enlarges TextBox when text width changes.
            if (TriggerTB.Text.Length > 0)
                TriggerTB.Width = TriggerTB.Text.Length * 8;

            // Removes white space if there's any.
            TriggerTB.Text = TriggerTB.Text.Replace(" ", "");

        }

        /// <summary>
        /// Disallows user to move through the textbox.
        /// </summary>
        /// <param name="sender">
        /// The triggering TextBox.
        /// </param>
        /// <param name="e">
        /// The pressed Key.
        /// </param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // if left arrow or right arrow are pressed
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:

                    // They won't work.
                    e.Handled = true;
                    break;
            }
        }

        #endregion

    }

}
