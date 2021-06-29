using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;


namespace WinFormsLab_NikitaAmol
{

    public partial class Form1 : Form
    {
        private enum Languages 
        { 
            ENGLISH,
            RUSSIAN
        }
        Languages langCode = Languages.ENGLISH; // set english as default
        Bitmap DrawArea;
        private static int wBigRect = 300;
        private static int hBigRect = 500;
        private static int wSmallRect = 30;
        private static readonly Color defaultCoverColor = Color.LightBlue;
        private static Color currentCoverColor = defaultCoverColor;
        private static readonly Color defaultTextColor = Color.Black;
        private static Color currentTextColor = defaultTextColor;
        private static string bookCoverTitle = "";
        private static string bookCoverAuthor = "";
        private static List<AdditionalText> additionalTexts = new List<AdditionalText>();
        private static AdditionalText currentAdditionalText;
        private static AdditionalText selectedAdditionalText;
        private static bool textSelected = false;
        private static bool textAdded = false;
        private static Point currentCursorPosition = new Point();
        private static bool doubleClickFlag = false;
        private static bool rightClickFlag = false;
        private static bool middleClickFlag = false;
        private static Point mouseDownLocation = new Point();
        public Form1()
        {
            InitializeComponent();
            
            splitContainer1.Panel2MinSize = 200;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            switch (langCode)
            {
                case Languages.ENGLISH:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;
                case Languages.RUSSIAN:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
                    break;
                default:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;
            }
            if (splitContainer1.Panel1.Width < splitContainer1.Panel2.Width)
            {
                int w = ClientRectangle.Width;
                int oldSplitterDistance = splitContainer1.SplitterDistance;
                int newSplitterDistance = w - oldSplitterDistance;
                splitContainer1.SplitterDistance = newSplitterDistance;
            }

            // adding menu items to File button:

            ToolStripMenuItem newB = new ToolStripMenuItem()
            {
                Name = "newB",
                Text = GlobalUIStrings.fileNewName
            };
            newB.Click += (sender, e) =>
            {
                ShowNewButtonDialog();
            };

            ToolStripMenuItem openB = new ToolStripMenuItem()
            {
                Name = "openB",
                Text = GlobalUIStrings.fileOpenName,
            };
            openB.Click += (sender, e) =>
            {
                // logic for opening a file

                Stream stream;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text files(*.TXT)|*.TXT";
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if((stream = openFileDialog.OpenFile()) != null)
                    {
                        OpenInfoFile(openFileDialog.FileName, stream);
                        stream.Close();
                    }
                    pictureBox1.Invalidate();
                    DrawRectangles(currentCoverColor);
                }

            };
            ToolStripMenuItem saveB = new ToolStripMenuItem()
            {
                Name = "saveB",
                Text = GlobalUIStrings.fileSaveName
            };
            saveB.Click += (sender, e) =>
            {
                // logic to save a file

                Stream stream;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text File(*.TXT) | *.TXT";
                if(saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != "")
                {
                    if ((stream = saveFileDialog.OpenFile()) != null)
                    {
                        CreateInfoFile(saveFileDialog.FileName, stream);
                        stream.Close();
                    }
                }
            };

            //adding shortcuts to buttons

            newB.ShortcutKeys = (Keys)Shortcut.CtrlN;
            openB.ShortcutKeys = (Keys)Shortcut.CtrlO;
            saveB.ShortcutKeys = (Keys)Shortcut.CtrlS;

            //adding buttons to File

            fileMenuItem.DropDownItems.Add(newB);
            fileMenuItem.DropDownItems.Add(openB);
            fileMenuItem.DropDownItems.Add(saveB);

            // adding menu items to Settings button:

            ToolStripMenuItem languageMenuItem = new ToolStripMenuItem()
            {
                Name = "languageMenuItem",
                Text = GlobalUIStrings.setLangName
            };
            ToolStripMenuItem enMenuItem = new ToolStripMenuItem()
            {
                Name = "enMenuItem",
                Text = GlobalUIStrings.englishName,
                Checked = false,
            };
            ToolStripMenuItem ruMenuItem = new ToolStripMenuItem()
            {
                Name = "ruMenuItem",
                Text = GlobalUIStrings.rusName,
                Checked = false
            };
            
            if (langCode == Languages.ENGLISH)
                enMenuItem.Checked = true;
            else
                ruMenuItem.Checked = true;
            enMenuItem.Click += (sender, e) =>
            {
                
                if (enMenuItem.Checked == true)
                {
                    return;
                }
                else
                {
                    enMenuItem.Checked = true;
                    ruMenuItem.Checked = false;
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    langCode = Languages.ENGLISH;
                    this.Controls.Clear();
                    this.InitializeComponent();
                    Form1_Load(null, EventArgs.Empty);
                }
            };
            ruMenuItem.Click += (sender, e) =>
            {
                
                if (ruMenuItem.Checked == true)
                {
                    return;
                }    
                else
                {
                    ruMenuItem.Checked = true;
                    enMenuItem.Checked = false;
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
                    langCode = Languages.RUSSIAN;
                    this.Controls.Clear();
                    this.InitializeComponent();
                    Form1_Load(null, EventArgs.Empty);
                }
            };
            languageMenuItem.DropDownItems.Add(enMenuItem);
            languageMenuItem.DropDownItems.Add(ruMenuItem);
            settingsMenuItem.DropDownItems.Add(languageMenuItem);
        }


        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            DrawRectangles(currentCoverColor); 
        }

        private void addTextButton_Click(object sender, EventArgs e)
        {
            ShowAddTextDialog("", HorizontalAlignment.Left, 16);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
            if (pictureBox1.Cursor == Cursors.Cross)
            {
                pictureBox1.Invalidate();
            }
        }

        public void ShowNewButtonDialog()
        {
            // creating a new form for dialog box

            Form d = new Form();
            d.FormBorderStyle = FormBorderStyle.FixedDialog;
            d.MinimizeBox = false;
            d.MaximizeBox = false;
            d.StartPosition = FormStartPosition.CenterScreen;
            d.Width = 350;
            d.Height = 350;
            d.Text = GlobalUIStrings.newCoverDialogName;

            // creating panel to divide dialog box into 4 squares

            TableLayoutPanel table = new TableLayoutPanel();
            table.Name = "newBTable";
            table.RowCount = 4;
            table.ColumnCount = 2;
            table.Dock = DockStyle.Fill;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));


            // creating controls on the left

            Label widthTextLabel = new Label()
            {
                Text = GlobalUIStrings.newBigWidth,
                Anchor = (AnchorStyles.Top |
                AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Label heightTextLabel = new Label()
            {
                Text = GlobalUIStrings.newBigHeight,
                Anchor = (AnchorStyles.Top | AnchorStyles.Bottom
                | AnchorStyles.Left | AnchorStyles.Right),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Label spineWidthTextLabel = new Label()
            {
                Text = GlobalUIStrings.newSmallWidth,
                Anchor = (AnchorStyles.Top | AnchorStyles.Bottom
                | AnchorStyles.Left | AnchorStyles.Right),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button cancelButton = new Button()
            {
                Text = GlobalUIStrings.cancelButton,
                Anchor = AnchorStyles.None,
                Width = 80,
                Height = 30,
            };
            cancelButton.Click += (sender, e) => { d.Close(); };

            // creating controls on the right side of the dialog box
            
            NumericUpDown widthNumericUpDown = new NumericUpDown()
            {
                Anchor = AnchorStyles.None,
                Width = 130,
                Height = 27,
                Increment = 1,
                Minimum = 0,
                Maximum = pictureBox1.Width,
                Value = wBigRect
            };
            NumericUpDown heightNumericUpDown = new NumericUpDown()
            {
                Anchor = AnchorStyles.None,
                Width = 130,
                Height = 27,
                Increment = 1,
                Minimum = 0,
                Maximum = pictureBox1.Height,
                Value = hBigRect
            };
            NumericUpDown widthSmallNumericUpDown = new NumericUpDown()
            {
                Anchor = AnchorStyles.None,
                Width = 130,
                Height = 27,
                Increment = 1,
                Minimum = 0,
                Maximum = pictureBox1.Height / 10,
                Value = wSmallRect
            };
            Button OKButton = new Button()
            {
                Text = "OK",
                Anchor = AnchorStyles.None,
                Width = 80,
                Height = 30,
            };
            OKButton.Click += (sender, e) =>
            {
                wBigRect = Convert.ToInt32(widthNumericUpDown.Value);
                hBigRect = Convert.ToInt32(heightNumericUpDown.Value);
                wSmallRect = Convert.ToInt32(widthSmallNumericUpDown.Value);
                DrawRectangles(defaultCoverColor); // since LightBlue is a default color
                currentTextColor = defaultTextColor;
                bookCoverTitle = titleTextBox.Text = "";
                bookCoverAuthor = authorTextBox.Text = "";
                
                additionalTexts = new List<AdditionalText>();
                d.Close();
            };
            //adding everything to our form 

            table.Controls.Add(widthTextLabel, 0, 0);
            table.Controls.Add(heightTextLabel, 0, 1);
            table.Controls.Add(spineWidthTextLabel, 0, 2);
            table.Controls.Add(cancelButton,0,3);
            table.Controls.Add(widthNumericUpDown, 1, 0);
            table.Controls.Add(heightNumericUpDown, 1, 1);
            table.Controls.Add(widthSmallNumericUpDown, 1, 2);
            table.Controls.Add(OKButton, 1, 3);
            d.Controls.Add(table);
            
            d.ShowDialog();
        }

        private void DrawRectangles(Color color)
        {
            // each time left panel is resized, rectangle is redrawn in the center of the panel

            DrawArea = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = DrawArea;

            // x and y coordinates of the center of the rectangle

            int xCenter = splitContainer1.Panel1.Width / 2;
            int yCenter = splitContainer1.Panel2.Height / 2;

            int hSmallRect = hBigRect;

            Graphics graphics;
            SolidBrush brush = new SolidBrush(color);
            graphics = Graphics.FromImage(DrawArea);

            // centering the rectangles

            int X = xCenter - (2 * wBigRect + wSmallRect) / 2;
            int Y = yCenter - hBigRect / 2;
            graphics.DrawRectangle(Pens.DarkGray, X, Y, wBigRect, hBigRect);
            graphics.FillRectangle(brush, X+1, Y+1, wBigRect-1, hBigRect-1);
            graphics.DrawRectangle(Pens.DarkGray, X + wBigRect, Y, wSmallRect, hSmallRect);
            graphics.FillRectangle(brush, X + wBigRect + 1, Y + 1, wSmallRect - 1, hSmallRect - 1);
            graphics.DrawRectangle(Pens.DarkGray, X + wBigRect + wSmallRect, Y, wBigRect, hBigRect);
            graphics.FillRectangle(brush, X + wBigRect + wSmallRect + 1, Y + 1, wBigRect - 1, hBigRect - 1);
        }

        private void changeBackgroundButton_Click(object sender, EventArgs e)
        {
            // logic for changing background color

            ColorDialog colorDialog = new ColorDialog();
            if(colorDialog.ShowDialog() == DialogResult.OK)
            {
                // change book background color
                currentCoverColor = colorDialog.Color;
                DrawRectangles(currentCoverColor);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // setting current text color

            SolidBrush brush = new SolidBrush(currentTextColor);

            // drawing Title

            int titleFontSize = 32;
            Font titleFont = new Font("Arial", titleFontSize);
            Graphics g = e.Graphics;
            SizeF titleRectangle = g.MeasureString(bookCoverTitle, titleFont);

            // decrease font if it is bigger than rectangle width or

            while(titleRectangle.Width > wBigRect || titleRectangle.Height > (hBigRect/3))
            {
                titleFont = new Font("Arial", --titleFontSize);
                titleRectangle = g.MeasureString(bookCoverTitle, titleFont);
            }
            int xPosTitle = Convert.ToInt32(pictureBox1.Width - (pictureBox1.Width - (2 * wBigRect + wSmallRect)) / 2 -
                            wBigRect / 2 - titleRectangle.Width / 2);
            int yPosTitle = Convert.ToInt32(pictureBox1.Height - ((pictureBox1.Height - hBigRect) / 2) -
                ((2 * hBigRect) / 3) - (hBigRect / 6) - titleRectangle.Height / 2);
            g.DrawString(bookCoverTitle, titleFont, brush, new Point(xPosTitle, yPosTitle));

            // drawing Author

            int authorFontSize = 24;
            Font authorFont = new Font("Arial", authorFontSize);
            SizeF authorRectangle = g.MeasureString(bookCoverAuthor, authorFont);
            while(authorRectangle.Width > wBigRect || authorRectangle.Height > (hBigRect/6))
            {
                authorFont = new Font("Arial", --authorFontSize);
                authorRectangle = g.MeasureString(bookCoverAuthor, authorFont);
            }
            int xPosAuthor = Convert.ToInt32(pictureBox1.Width - (pictureBox1.Width - (2 * wBigRect + wSmallRect)) / 2 -
            wBigRect / 2 - authorRectangle.Width / 2);
            int yPosAuthor = Convert.ToInt32(pictureBox1.Height - ((pictureBox1.Height - hBigRect) / 2) -
            (hBigRect / 2) - (hBigRect / 4) + titleRectangle.Height / 2 - authorRectangle.Height / 2);
            
            g.DrawString(bookCoverAuthor, authorFont, brush, new Point(xPosAuthor, yPosAuthor));


            // drawing rotated Title

            int rotatedTitleFontSize = 32;
            Font rotatedTitleFont = new Font("Arial", rotatedTitleFontSize);
            SizeF rotatedTitleRect = g.MeasureString(bookCoverTitle, rotatedTitleFont);
            while(rotatedTitleRect.Width > hBigRect/2 || rotatedTitleRect.Height > wSmallRect)
            {
                rotatedTitleFont = new Font("Arial", --rotatedTitleFontSize);
                rotatedTitleRect = g.MeasureString(bookCoverTitle, rotatedTitleFont);
            }
            int xPosRotatedTitle = Convert.ToInt32(pictureBox1.Width - (pictureBox1.Width - (2 * wBigRect + wSmallRect)) / 2 - wBigRect
                - (wSmallRect / 2) - (rotatedTitleRect.Height / 2));
            int yPosRotatedTitle = Convert.ToInt32(pictureBox1.Height - (pictureBox1.Height - hBigRect)/2  - (hBigRect/4) 
                + (rotatedTitleRect.Width/2));
            System.Drawing.Drawing2D.GraphicsState state = g.Save();
            g.ResetTransform();
            g.RotateTransform(-90); // rotation
            g.TranslateTransform(xPosRotatedTitle, yPosRotatedTitle, System.Drawing.Drawing2D.MatrixOrder.Append);
            g.DrawString(bookCoverTitle, rotatedTitleFont, brush, 0, 0);
            g.Restore(state);

            // drawing rotated Author

            int rotatedAuthorFontSize = 24;
            Font rotatedAuthorFont = new Font("Arial", rotatedAuthorFontSize);
            SizeF rotatedAuthorRect = g.MeasureString(bookCoverAuthor, rotatedAuthorFont);
            while (rotatedAuthorRect.Width > hBigRect / 2 || rotatedAuthorRect.Height > wSmallRect)
            {
                rotatedAuthorFont = new Font("Arial", --rotatedAuthorFontSize);
                rotatedAuthorRect = g.MeasureString(bookCoverAuthor, rotatedAuthorFont);
            }
            int xPosRotatedAuthor = Convert.ToInt32(pictureBox1.Width - (pictureBox1.Width - (2 * wBigRect 
                + wSmallRect)) / 2 - wBigRect  - (wSmallRect / 2) - (rotatedAuthorRect.Height / 2));
            int yPosRotatedAuthor = Convert.ToInt32(pictureBox1.Height - (pictureBox1.Height - hBigRect) / 2 
                - (3*hBigRect / 4) + (rotatedAuthorRect.Width / 2));
            state = g.Save();
            g.ResetTransform();
            g.RotateTransform(-90); // rotation
            g.TranslateTransform(xPosRotatedAuthor, yPosRotatedAuthor, System.Drawing.Drawing2D.MatrixOrder.Append);
            string withoutSpaces = string.Join(" ", bookCoverAuthor.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            // removing 

            g.DrawString(withoutSpaces, rotatedAuthorFont, brush, 0, 0);
            g.Restore(state);

            // draw texts and set cursor to default

            int x = (pictureBox1.Width - wBigRect) / 2;
            int y = (pictureBox1.Height - hBigRect) / 2;
            Point topLeftCover = new Point(x, y); // current top left corner of the cover
                                                  // drawing all additional texts already written
            if (additionalTexts.Count == 0)
                return;

            // redrawing each time selected string

            if (middleClickFlag == true)
            {
                // it means that middle mouse button is pressed

                foreach (AdditionalText t in additionalTexts)
                {
                    if (t == selectedAdditionalText)
                    {
                        Font f = new Font("Arial", t.Size);
                        g.DrawString(t.Text, f, brush, t.Point);
                        SizeF textSize = g.MeasureString(t.Text, f);
                        Size rectSize = new Size(Convert.ToInt32(textSize.Width), Convert.ToInt32(textSize.Height));
                        Pen pen = new Pen(InvertColour(currentCoverColor));
                        //draw a rectangle around this string
                        Rectangle border = new Rectangle(t.Point, rectSize);
                        g.DrawRectangle(pen, border);
                    }
                }
            }

            //drawing texts when right is clicked

            foreach (AdditionalText t in additionalTexts)
            {
                Font f = new Font("Arial", t.Size);
                int posX = 0;
                int posY = 0;
                posX = topLeftCover.X + t.DistanceX;
                posY = topLeftCover.Y + t.DistanceY;
                
                SizeF textSize = g.MeasureString(t.Text, f);
                Point textPos = new Point(Convert.ToInt32(posX - (textSize.Width/2)),
                    Convert.ToInt32(posY - (textSize.Height/2)));
                t.Point = textPos;

                g.DrawString(t.Text, f, brush, t.Point);
            }
            textAdded = false;

            // reacting to doubleClick

            if (doubleClickFlag)
            {
                Point cursorPos = currentCursorPosition;
                AdditionalText chosenText = new AdditionalText();
                bool flag = false;

                // find a text which was double clicked

                foreach (AdditionalText t in additionalTexts)
                {
                    Font f = new Font("Arial", t.Size);
                    SizeF textSize = g.MeasureString(t.Text, f);
                    flag = cursorIsInRectRange(cursorPos.X, cursorPos.Y, t.Point.X, t.Point.Y,
                        Convert.ToInt32(textSize.Width), Convert.ToInt32(textSize.Height));
                    if (flag)
                    {
                        chosenText = t;
                        break;
                    }
                }
                if (flag) // display a dialogbox with text, alignment and font size
                    ShowAddTextDialog(chosenText.Text, chosenText.Alignment, chosenText.Size);


                doubleClickFlag = false;
            }

            // selecting string with rightClick flag

            if (rightClickFlag == true)
            {
                Point cursorPos = currentCursorPosition;
                AdditionalText chosenText = new AdditionalText();
                Point textPos = new Point();
                Font f = new Font("Arial",16);
                bool found = false;
                // find string which was rightclicked
                foreach (AdditionalText t in additionalTexts)
                {
                    f = new Font("Arial", t.Size);
                    SizeF textSize = g.MeasureString(t.Text, f);
                     int posX = 0;
                     int posY = 0;
                    posX = x + t.DistanceX;
                    posY = y + t.DistanceY;

                     textPos = new Point(Convert.ToInt32(posX - (textSize.Width / 2)),
                     Convert.ToInt32(posY - (textSize.Height / 2)));
                     found = cursorIsInRectRange(cursorPos.X, cursorPos.Y, textPos.X, textPos.Y,
                         Convert.ToInt32(textSize.Width), Convert.ToInt32(textSize.Height));
                    if(found)
                    {
                        textSelected = true;
                        selectedAdditionalText = t;
                        chosenText = t;
                        break;
                    }
                }
                if(found)
                {
                    SizeF textSize = g.MeasureString(chosenText.Text, f);
                    Size rectSize = new Size(Convert.ToInt32(textSize.Width), Convert.ToInt32(textSize.Height));
                    Pen pen = new Pen(InvertColour(currentCoverColor));

                    //draw a rectangle around this string

                    Rectangle border = new Rectangle(textPos, rectSize);
                    g.DrawRectangle(pen,border);
                }
                else
                {
                    textSelected = false;
                }
                rightClickFlag = false;
            }
            pictureBox1.Cursor = Cursors.Default;
        }

        private void titleTextBox_TextChanged(object sender, EventArgs e)
        {
            // logic to draw a string with title on the book cover

            bookCoverTitle = titleTextBox.Text;
            pictureBox1.Invalidate();
        }

        private void authorTextBox_TextChanged(object sender, EventArgs e)
        {
            bookCoverAuthor = authorTextBox.Text;
            pictureBox1.Invalidate();
        }

        private void changeTextButton_Click(object sender, EventArgs e)
        {
            // changing color of the text 

            ColorDialog colorDialog = new ColorDialog();
            if(colorDialog.ShowDialog() == DialogResult.OK)
            {
                currentTextColor = colorDialog.Color;
                pictureBox1.Invalidate();
            }
        }
        private void ShowAddTextDialog(string text, HorizontalAlignment a, int fontSize)
        {
            // creating a new form for dialog box

            Form d = new Form();
            d.FormBorderStyle = FormBorderStyle.FixedDialog;
            d.MinimizeBox = false;
            d.MaximizeBox = false;
            d.StartPosition = FormStartPosition.CenterScreen;
            d.Width = 500;
            d.Height = 400;
            d.Text = GlobalUIStrings.addDialogBoxName;

            // creating main tablelayoutpanel with 3 rows and 1 column

            TableLayoutPanel mainTable = new TableLayoutPanel();
            mainTable.Name = "mainTable";
            mainTable.RowCount = 3;
            mainTable.ColumnCount = 1;
            mainTable.Dock = DockStyle.Fill;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            // creating 2 child tables for top and bottom cells with 1 row and 2 columns

            TableLayoutPanel topChildTable = new TableLayoutPanel();
            TableLayoutPanel bottomChildTable = new TableLayoutPanel();
            topChildTable.Name = "topChildTable";
            topChildTable.RowCount = 1;
            topChildTable.ColumnCount = 2;
            topChildTable.Dock = DockStyle.Fill;
            topChildTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            topChildTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            topChildTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            bottomChildTable.Name = "bottomChildTable";
            bottomChildTable.RowCount = 1;
            bottomChildTable.ColumnCount = 2;
            bottomChildTable.Dock = DockStyle.Fill;
            bottomChildTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            bottomChildTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            bottomChildTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // creating child table at the top left corner of the topChildTable

            TableLayoutPanel topLeftChildTable = new TableLayoutPanel();
            topLeftChildTable.Name = "topLeftChildTable";
            topLeftChildTable.RowCount = 1;
            topLeftChildTable.ColumnCount = 2;
            topLeftChildTable.Dock = DockStyle.Fill;
            topLeftChildTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            topLeftChildTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            topLeftChildTable.RowStyles.Add(new ColumnStyle(SizeType.Percent, 100F));


            // creating Font Size label at the top left left corner

            Label fontSizeLabel = new Label()
            {
                Name = "fontSizeLabel",
                Text = GlobalUIStrings.addDialogFontSizeLabel,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            NumericUpDown fontSizeNumericUpDown = new NumericUpDown()
            {
                Name = "fontSizeNumericUpDown",
                Value = fontSize,
                Maximum = 32,
                Minimum = 1,
                Increment = 1,
                Anchor = AnchorStyles.None
            };

            // creating checkbox at the top left right corner

            GroupBox textAlignmentGBox = new GroupBox()
            {
                Name = "textAlignmentGBox",
                Text = GlobalUIStrings.addDialogGBoxName,
                Dock = DockStyle.Fill
            };
            RadioButton lRadioButton = new RadioButton()
            {
                Name = "lRadioButton",
                Text = GlobalUIStrings.lRadio,
                AutoSize = true,
                Top = textAlignmentGBox.Top + 25,
                Left = textAlignmentGBox.Left + 5,
                Checked = false
            };

            RadioButton cRadioButton = new RadioButton()
            {
                Name = "cRadioButton",
                Text = GlobalUIStrings.cRadio,
                AutoSize = true,
                Top = textAlignmentGBox.Top + 25 + lRadioButton.Height,
                Left = textAlignmentGBox.Left + 5
            };
            RadioButton rRadioButton = new RadioButton()
            {
                Name = "rRadioButton",
                Text = GlobalUIStrings.rRadio,
                AutoSize = true,
                Top = textAlignmentGBox.Top + 25 + 2 * cRadioButton.Height,
                Left = textAlignmentGBox.Left + 5
            };


            textAlignmentGBox.Controls.Add(lRadioButton);
            textAlignmentGBox.Controls.Add(cRadioButton);
            textAlignmentGBox.Controls.Add(rRadioButton);

            //text box inbetween top and bottom mainTable panels

            TextBox addedTextTBox = new TextBox()
            {
                Text = text,
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical
            };
            // selecting text when diaolog box is opened

            addedTextTBox.GotFocus += (sender, e) =>
            {
                if (!String.IsNullOrEmpty(addedTextTBox.Text))
                {
                    addedTextTBox.SelectionStart = 0;
                    addedTextTBox.SelectionLength = addedTextTBox.Text.Length;
                }
            };


            if (a == HorizontalAlignment.Left)
            {
                lRadioButton.Checked = true;
                addedTextTBox.TextAlign = HorizontalAlignment.Left;
            }
            else if (a == HorizontalAlignment.Center)
            {
                cRadioButton.Checked = true;
                addedTextTBox.TextAlign = HorizontalAlignment.Center;
            }
            else if (a == HorizontalAlignment.Right)
            {
                rRadioButton.Checked = true;
                addedTextTBox.TextAlign = HorizontalAlignment.Right;
            }

            lRadioButton.Click += (sender, e) =>
            {
                addedTextTBox.TextAlign = HorizontalAlignment.Left;
            };
            cRadioButton.Click += (sender, e) =>
            {
                addedTextTBox.TextAlign = HorizontalAlignment.Center;
            };
            rRadioButton.Click += (sender, e) =>
            {
                addedTextTBox.TextAlign = HorizontalAlignment.Right;
            };

            // creating cancel button

            Button cancelButton = new Button()
            {
                Text = GlobalUIStrings.cancelButton,
                Anchor = AnchorStyles.None,
                Width = 80,
                Height = 30,
            };
            cancelButton.Click += (sender, e) => { d.Close(); };

            //creating OK button

            Button OKButton = new Button()
            {
                Text = "OK",
                Anchor = AnchorStyles.None,
                Width = 80,
                Height = 30,
            };
            OKButton.Click += (sender, e) =>
            {
                if (doubleClickFlag)
                {
                    foreach (AdditionalText t in additionalTexts)
                    {
                        if (text == t.Text) // found text to change
                        {
                            t.Text = addedTextTBox.Text;
                            t.Alignment = addedTextTBox.TextAlign;
                            t.Size = Convert.ToInt32(fontSizeNumericUpDown.Value);
                            pictureBox1.Invalidate();
                            break;
                        }
                    }
                }
                else
                {
                    currentAdditionalText = new AdditionalText();
                    textAdded = true;
                    if (lRadioButton.Checked == true)
                        currentAdditionalText.Alignment = HorizontalAlignment.Left;
                    else if (cRadioButton.Checked == true)
                        currentAdditionalText.Alignment = HorizontalAlignment.Center;
                    else
                        currentAdditionalText.Alignment = HorizontalAlignment.Right;

                    currentAdditionalText.Text = addedTextTBox.Text;
                    currentAdditionalText.Size = Convert.ToInt32(fontSizeNumericUpDown.Value);
                }

                d.Close();
            };

            topLeftChildTable.Controls.Add(fontSizeLabel, 0, 0);
            topLeftChildTable.Controls.Add(fontSizeNumericUpDown, 1, 0);
            topChildTable.Controls.Add(topLeftChildTable, 0, 0);
            topChildTable.Controls.Add(textAlignmentGBox, 1, 0);
            bottomChildTable.Controls.Add(cancelButton, 0, 0);
            bottomChildTable.Controls.Add(OKButton, 1, 0);
            mainTable.Controls.Add(topChildTable, 0, 0);
            mainTable.Controls.Add(addedTextTBox, 0, 1);
            mainTable.Controls.Add(bottomChildTable, 0, 2);
            d.Controls.Add(mainTable);
            d.Activated += (sender, e) =>
            {
                addedTextTBox.Focus();
            };
            d.ShowDialog();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            currentCursorPosition = e.Location;
            if(e.Button == MouseButtons.Middle && textSelected == true)
            {
                // repaint if middle mouse button is pressed 
                foreach(AdditionalText t in additionalTexts)
                {
                    if(t == selectedAdditionalText)
                    {
                        selectedAdditionalText.Point = t.Point = new Point(e.X + t.Point.X - mouseDownLocation.X, e.Y + t.Point.Y - mouseDownLocation.Y);
                        break;
                    }
                }
                pictureBox1.Invalidate();
            }
            if(textAdded == true)
            {
                pictureBox1.Cursor = Cursors.Cross;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            currentCursorPosition = e.Location;
            if(e.Button == MouseButtons.Middle && textSelected == true)
            {
                int x = (pictureBox1.Width - wBigRect) / 2;
                int y = (pictureBox1.Height - hBigRect) / 2;
                Point topLeftCover = new Point(x, y); // current top left corner of the cover
                selectedAdditionalText.DistanceX = e.X - topLeftCover.X;
                selectedAdditionalText.DistanceY = e.Y - topLeftCover.Y;
                selectedAdditionalText = new AdditionalText();
                middleClickFlag = false;
                pictureBox1.Invalidate();
            }
            
            if(e.Button == MouseButtons.Right)
            {
                rightClickFlag = true;
                pictureBox1.Invalidate();
            }
            if (textAdded == true)
            {
                // make the points relative to the top left corner of the bound

                int x = (pictureBox1.Width - wBigRect) / 2;
                int y = (pictureBox1.Height - hBigRect) / 2;
                Point topLeftCover = new Point(x, y); // current top left corner of the cover
                currentAdditionalText.DistanceX = e.X - topLeftCover.X;
                currentAdditionalText.DistanceY = e.Y - topLeftCover.Y;
                currentAdditionalText.Point = new Point(e.X, e.Y);
                additionalTexts.Add(currentAdditionalText);
            }            
        
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            DrawRectangles(currentCoverColor);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if(me.Button == MouseButtons.Left)
            {
                doubleClickFlag = true;
                pictureBox1.Invalidate();
            }
        }
        private bool cursorIsInRectRange(int cursorX, int cursorY,int rectX, int rectY,int rectWidth, int rectHeight)
        {
            int maxX = rectX + rectWidth;
            int maxY = rectY + rectHeight;
            if (cursorX > rectX && cursorY > rectY && cursorX < maxX && cursorY < maxY)
                return true;
            else
                return false;
        }
        private Color InvertColour(Color ColourToInvert)
        {
            return Color.FromArgb(255 - ColourToInvert.R,
              255 - ColourToInvert.G, 255 - ColourToInvert.B);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && textSelected == true)
            {
                middleClickFlag = true;
                mouseDownLocation = e.Location;
                pictureBox1.Invalidate();
            }
        }
        private void CreateInfoFile(string fileName, Stream stream)
        {
            /* The content of a file will look like this:
             Author
             Title
             wBigWidth
             wSmallWidth
             wBigHeight
             BackgroundColor.R
             BackgroundColor.G
             BackgroundColor.B
             TextColor.R
             TextColor.G
             TextColor.B
             Alignment
             PointX
             PointY
             DistanceX
             DistanceY
             fontSize
             Text
             */
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(bookCoverAuthor);
            sw.WriteLine(bookCoverTitle);
            sw.WriteLine(wBigRect);
            sw.WriteLine(wSmallRect);
            sw.WriteLine(hBigRect);
            sw.WriteLine(currentCoverColor.R);
            sw.WriteLine(currentCoverColor.G);
            sw.WriteLine(currentCoverColor.B);
            sw.WriteLine(currentTextColor.R);
            sw.WriteLine(currentTextColor.G);
            sw.WriteLine(currentTextColor.B);
            foreach(AdditionalText t in additionalTexts)
            {
                //firstly alignment
                if (t.Alignment == HorizontalAlignment.Left)
                {
                    sw.WriteLine("a0"); // 0 - left align
                }
                else if (t.Alignment == HorizontalAlignment.Center)
                {
                    sw.WriteLine("a1"); // 1 - center align
                }
                else
                    sw.WriteLine("a2"); // 2 - right
                sw.WriteLine(t.Point.X);
                sw.WriteLine(t.Point.Y);
                sw.WriteLine(t.DistanceX);
                sw.WriteLine(t.DistanceY);
                sw.WriteLine(t.Size);
                sw.WriteLine(t.Text);
            }
            sw.Close();
        }
        private void OpenInfoFile(string fileName, Stream stream)
        {
            try {

                StreamReader sr = new StreamReader(stream);
                bookCoverAuthor = sr.ReadLine();
                bookCoverTitle = sr.ReadLine();
                wBigRect = Convert.ToInt32(sr.ReadLine());
                wSmallRect = Convert.ToInt32(sr.ReadLine());
                hBigRect = Convert.ToInt32(sr.ReadLine());
                byte cR, cG, cB;
                cR = Convert.ToByte(sr.ReadLine());
                cG = Convert.ToByte(sr.ReadLine());
                cB = Convert.ToByte(sr.ReadLine());
                currentCoverColor = Color.FromArgb(cR, cG, cB);
                byte tR, tG, tB;
                tR = Convert.ToByte(sr.ReadLine());
                tG = Convert.ToByte(sr.ReadLine());
                tB = Convert.ToByte(sr.ReadLine());
                currentTextColor = Color.FromArgb(tR, tG, tB);
                additionalTexts = new List<AdditionalText>();

                string line = sr.ReadLine();
                while (line != null)
                {
                    AdditionalText instance = new AdditionalText();
                    if (line == "a0")
                        instance.Alignment = HorizontalAlignment.Left;
                    else if (line == "a1")
                        instance.Alignment = HorizontalAlignment.Center;
                    else if (line == "a2")
                        instance.Alignment = HorizontalAlignment.Right;

                    int x, y;
                    line = sr.ReadLine();
                    x = Convert.ToInt32(line);
                    line = sr.ReadLine();
                    y = Convert.ToInt32(line);
                    instance.Point = new Point(x, y);

                    line = sr.ReadLine();
                    instance.DistanceX = Convert.ToInt32(line);
                    line = sr.ReadLine();
                    instance.DistanceY = Convert.ToInt32(line);

                    line = sr.ReadLine();
                    instance.Size = Convert.ToInt32(line);
                    line = sr.ReadLine();
                    instance.Text = line;

                    additionalTexts.Add(instance);
                    line = sr.ReadLine();
                }

                sr.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show("Exception was catched.");
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete && textSelected == true)
            {
                AdditionalText tmp = new AdditionalText();
                foreach(AdditionalText t in additionalTexts)
                {
                    if(t == selectedAdditionalText)
                    {
                        tmp = t;
                        break;
                    }
                }
                additionalTexts.Remove(tmp);
                pictureBox1.Invalidate();
            }
        }
    }

    public class AdditionalText
    {
        public string Text { get; set; }
        public Point Point { get; set; }
        public int Size { get; set; } // font size
        public int DistanceX { get; set; }
        public int DistanceY { get; set; }
        public HorizontalAlignment Alignment { get; set; }
    }
}
