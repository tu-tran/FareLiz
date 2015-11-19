namespace SkyDean.FareLiz.WinForm.Components.Controls.Grid
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>Extends PropertyGrid to provide a changable description area height.</summary>
    public class ExPropertyGrid : PropertyGrid
    {
        /// <summary>
        /// The description area line count.
        /// </summary>
        private int descriptionAreaLineCount = 2;

        /// <summary>
        /// The doc comment.
        /// </summary>
        private Control docComment;

        /// <summary>
        /// The doc comment type.
        /// </summary>
        private Type docCommentType;

        /// <summary>
        /// The lines property.
        /// </summary>
        private PropertyInfo linesProperty;

        /// <summary>
        /// The property grid view.
        /// </summary>
        private Control propertyGridView;

        /// <summary>
        /// The size change is from user.
        /// </summary>
        private bool sizeChangeIsFromUser = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExPropertyGrid"/> class. Initializes a new instance of the CustomPropertyGrid class.
        /// </summary>
        public ExPropertyGrid()
        {
            foreach (Control control in this.Controls)
            {
                Type controlType = control.GetType();
                if (controlType.Name == "DocComment")
                {
                    this.docCommentType = controlType;
                    this.docComment = control;
                    this.linesProperty = this.docCommentType.GetProperty("Lines");
                    FieldInfo userSizedField = this.docCommentType.BaseType.GetField("userSized", BindingFlags.Instance | BindingFlags.NonPublic);
                    userSizedField.SetValue(this.docComment, true);
                }
                else if (controlType.Name == "PropertyGridView")
                {
                    this.propertyGridView = control;
                }
            }

            this.docComment.SizeChanged += this.HandleDocCommentSizeChanged;
        }

        /// <summary>Gets or sets the description area line count.</summary>
        /// <value>The description area line count.</value>
        /// <exception cref="ArgumentException"> If value is less than zero.</exception>
        /// <exception cref="TypeLoadException"> If not of the all objects required to set the field were found.</exception>
        public int DescriptionAreaLineCount
        {
            get
            {
                return this.descriptionAreaLineCount;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The value cannot be less than zero.");
                }

                if (this.docCommentType == null || this.docComment == null || this.propertyGridView == null || this.linesProperty == null)
                {
                    throw new TypeLoadException("Not all of the objects required to set the field were found.");
                }

                try
                {
                    int oldDocCommentHeight = this.docComment.Height;
                    int oldValue = this.DescriptionAreaLineCount;
                    this.linesProperty.SetValue(this.docComment, value, null);
                    int difference = this.docComment.Height - oldDocCommentHeight;
                    if (this.docComment.Top - difference > this.propertyGridView.Top)
                    {
                        this.sizeChangeIsFromUser = false;
                        this.propertyGridView.Height -= difference;
                        this.docComment.Top -= difference;
                        this.descriptionAreaLineCount = value;
                        this.sizeChangeIsFromUser = true;
                    }
                    else
                    {
                        this.linesProperty.SetValue(this.docComment, oldValue, null);
                    }
                }
                catch (TargetInvocationException)
                {
                }

                this.Refresh();
            }
        }

        /// <summary>Gets or sets the height of the description area.</summary>
        /// <value>The height of the description area.</value>
        public int DescriptionAreaHeight
        {
            get
            {
                return this.docComment.Height;
            }

            set
            {
                int difference = value - this.docComment.Height;
                if (this.docComment.Top - difference > this.propertyGridView.Top)
                {
                    this.docComment.Height = value;
                    this.docComment.Top -= difference;
                    this.propertyGridView.Height -= difference;
                    this.Refresh();
                }
            }
        }

        /// <summary>Occurs when the description area size is changed by the user.</summary>
        public event EventHandler UserChangedDescriptionAreaSize;

        /// <summary>
        /// Raises the UserChangedDescriptionAreaSize event.
        /// </summary>
        /// <param name="e">
        /// The System.EventArgs instance containing the event data.
        /// </param>
        protected void OnUserChangedDescriptionAreaSize(EventArgs e)
        {
            EventHandler handler = this.UserChangedDescriptionAreaSize;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Handles this.docComment.SizeChanged.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The System.EventArgs instance containing the event data.
        /// </param>
        private void HandleDocCommentSizeChanged(object sender, EventArgs e)
        {
            if (this.IsHandleCreated && this.sizeChangeIsFromUser)
            {
                try
                {
                    var lineVal = this.linesProperty.GetValue(this.docComment, null);
                    this.descriptionAreaLineCount = (int)lineVal;
                    this.OnUserChangedDescriptionAreaSize(EventArgs.Empty);
                }
                catch (TargetInvocationException)
                {
                }
            }
        }
    }
}