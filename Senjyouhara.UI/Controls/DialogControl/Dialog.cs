using Senjyouhara.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Senjyouhara.UI.Controls.DialogControl
{
    public class Dialog : ContentControl
    {
        #region Static proerties

        /// <summary>
        /// Property for <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
            typeof(object), typeof(Dialog), new PropertyMetadata(null));

        /// <summary>
        /// Property for <see cref="TitleTemplate"/>.
        /// </summary>
        public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(nameof(TitleTemplate),
            typeof(DataTemplate), typeof(Dialog), new PropertyMetadata(null));

        /// <summary>
        /// Property for <see cref="DialogWidth"/>.
        /// </summary>
        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register(nameof(DialogWidth),
                typeof(double), typeof(Dialog), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Property for <see cref="DialogHeight"/>.
        /// </summary>
        public static readonly DependencyProperty DialogHeightProperty =
            DependencyProperty.Register(nameof(DialogHeight),
                typeof(double), typeof(Dialog), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Property for <see cref="DialogMaxWidth"/>.
        /// </summary>
        public static readonly DependencyProperty DialogMaxWidthProperty =
            DependencyProperty.Register(nameof(DialogMaxWidth),
                typeof(double), typeof(Dialog), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Property for <see cref="DialogMaxHeight"/>.
        /// </summary>
        public static readonly DependencyProperty DialogMaxHeightProperty =
            DependencyProperty.Register(nameof(DialogMaxHeight),
                typeof(double), typeof(Dialog), new PropertyMetadata(double.PositiveInfinity));

        /// <summary>
        /// Property for <see cref="DialogMargin"/>.
        /// </summary>
        public static readonly DependencyProperty DialogMarginProperty =
            DependencyProperty.Register(nameof(DialogMargin),
                typeof(Thickness), typeof(Dialog));

        /// <summary>
        /// Property for <see cref="PrimaryButtonText"/>.
        /// </summary>
        public static readonly DependencyProperty PrimaryButtonTextProperty =
            DependencyProperty.Register(nameof(PrimaryButtonText),
                typeof(string), typeof(Dialog), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Property for <see cref="SecondaryButtonText"/>.
        /// </summary>
        public static readonly DependencyProperty SecondaryButtonTextProperty =
            DependencyProperty.Register(nameof(SecondaryButtonText),
                typeof(string), typeof(Dialog), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Property for <see cref="CloseButtonText"/>.
        /// </summary>
        public static readonly DependencyProperty CloseButtonTextProperty =
            DependencyProperty.Register(nameof(CloseButtonText),
                typeof(string), typeof(Dialog), new PropertyMetadata("Close"));

        /// <summary>
        /// Property for <see cref="PrimaryButtonIcon"/>.
        /// </summary>
        public static readonly DependencyProperty PrimaryButtonIconProperty =
            DependencyProperty.Register(nameof(PrimaryButtonIcon),
                typeof(ContentControl), typeof(Dialog), new PropertyMetadata(null));

        /// <summary>
        /// Property for <see cref="SecondaryButtonIcon"/>.
        /// </summary>
        public static readonly DependencyProperty SecondaryButtonIconProperty =
            DependencyProperty.Register(nameof(SecondaryButtonIcon),
                typeof(ContentControl), typeof(Dialog), new PropertyMetadata(null));

        /// <summary>
        /// Property for <see cref="CloseButtonIcon"/>.
        /// </summary>
        public static readonly DependencyProperty CloseButtonIconProperty =
            DependencyProperty.Register(nameof(CloseButtonIcon),
                typeof(ContentControl), typeof(Dialog), new PropertyMetadata(null));

        /// <summary>
        /// Property for <see cref="IsPrimaryButtonEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsPrimaryButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsPrimaryButtonEnabled),
                typeof(bool), typeof(Dialog), new PropertyMetadata(true));

        /// <summary>
        /// Property for <see cref="IsSecondaryButtonEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsSecondaryButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsSecondaryButtonEnabled),
                typeof(bool), typeof(Dialog), new PropertyMetadata(true));

        /// <summary>
        /// Property for <see cref="PrimaryButtonAppearance"/>.
        /// </summary>
        public static readonly DependencyProperty PrimaryButtonAppearanceProperty =
            DependencyProperty.Register(nameof(PrimaryButtonAppearance),
                typeof(ControlAppearance), typeof(Dialog), new PropertyMetadata(ControlAppearance.Primary));

        /// <summary>
        /// Property for <see cref="SecondaryButtonAppearance"/>.
        /// </summary>
        public static readonly DependencyProperty SecondaryButtonAppearanceProperty =
            DependencyProperty.Register(nameof(SecondaryButtonAppearance),
                typeof(ControlAppearance), typeof(Dialog), new PropertyMetadata(ControlAppearance.Secondary));

        /// <summary>
        /// Property for <see cref="CloseButtonAppearance"/>.
        /// </summary>
        public static readonly DependencyProperty CloseButtonAppearanceProperty =
            DependencyProperty.Register(nameof(CloseButtonAppearance),
                typeof(ControlAppearance), typeof(Dialog), new PropertyMetadata(ControlAppearance.Secondary));

        /// <summary>
        /// Property for <see cref="DefaultButton"/>.
        /// </summary>
        public static readonly DependencyProperty DefaultButtonProperty =
            DependencyProperty.Register(nameof(DefaultButton),
                typeof(DialogButton), typeof(Dialog), new PropertyMetadata(DialogButton.Primary));

        /// <summary>
        /// Property for <see cref="IsFooterVisible"/>.
        /// </summary>
        public static readonly DependencyProperty IsFooterVisibleProperty =
            DependencyProperty.Register(nameof(IsFooterVisible),
                typeof(bool), typeof(Dialog), new PropertyMetadata(true));

        /// <summary>
        /// Property for <see cref="TemplateButtonCommand"/>.
        /// </summary>
        public static readonly DependencyProperty TemplateButtonCommandProperty =
            DependencyProperty.Register(nameof(TemplateButtonCommand),
                typeof(IRelayCommand), typeof(Dialog), new PropertyMetadata(null));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title of the <see cref="Dialog"/>.
        /// </summary>
        public object Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets the title template of the <see cref="Dialog"/>.
        /// </summary>
        public DataTemplate TitleTemplate
        {
            get => (DataTemplate)GetValue(TitleTemplateProperty);
            set => SetValue(TitleTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the <see cref="Dialog"/>.
        /// </summary>
        public double DialogWidth
        {
            get => (double)GetValue(DialogWidthProperty);
            set => SetValue(DialogWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the height of the <see cref="Dialog"/>.
        /// </summary>
        public double DialogHeight
        {
            get => (double)GetValue(DialogHeightProperty);
            set => SetValue(DialogHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the max width of the <see cref="Dialog"/>.
        /// </summary>
        public double DialogMaxWidth
        {
            get => (double)GetValue(DialogMaxWidthProperty);
            set => SetValue(DialogMaxWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the max height of the <see cref="Dialog"/>.
        /// </summary>
        public double DialogMaxHeight
        {
            get => (double)GetValue(DialogMaxHeightProperty);
            set => SetValue(DialogMaxHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the margin of the <see cref="Dialog"/>.
        /// </summary>
        public Thickness DialogMargin
        {
            get => (Thickness)GetValue(DialogMarginProperty);
            set => SetValue(DialogMarginProperty, value);
        }

        /// <summary>
        /// Gets or sets the text to display on the primary button.
        /// </summary>
        public string PrimaryButtonText
        {
            get => (string)GetValue(PrimaryButtonTextProperty);
            set => SetValue(PrimaryButtonTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the text to be displayed on the secondary button.
        /// </summary>
        public string SecondaryButtonText
        {
            get => (string)GetValue(SecondaryButtonTextProperty);
            set => SetValue(SecondaryButtonTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the text to display on the close button.
        /// </summary>
        public string CloseButtonText
        {
            get => (string)GetValue(CloseButtonTextProperty);
            set => SetValue(CloseButtonTextProperty, value);
        }

        public ContentControl PrimaryButtonIcon
        {
            get => (ContentControl)GetValue(PrimaryButtonIconProperty);
            set => SetValue(PrimaryButtonIconProperty, value);
        }

        public ContentControl SecondaryButtonIcon
        {
            get => (ContentControl)GetValue(SecondaryButtonIconProperty);
            set => SetValue(SecondaryButtonIconProperty, value);
        }

        public ContentControl CloseButtonIcon
        {
            get => (ContentControl)GetValue(CloseButtonIconProperty);
            set => SetValue(CloseButtonIconProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> primary button is enabled.
        /// </summary>
        public bool IsPrimaryButtonEnabled
        {
            get => (bool)GetValue(IsPrimaryButtonEnabledProperty);
            set => SetValue(IsPrimaryButtonEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Dialog"/> secondary button is enabled.
        /// </summary>
        public bool IsSecondaryButtonEnabled
        {
            get => (bool)GetValue(IsSecondaryButtonEnabledProperty);
            set => SetValue(IsSecondaryButtonEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="ControlAppearance"/> to apply to the primary button.
        /// </summary>
        public ControlAppearance PrimaryButtonAppearance
        {
            get => (ControlAppearance)GetValue(PrimaryButtonAppearanceProperty);
            set => SetValue(PrimaryButtonAppearanceProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="ControlAppearance"/> to apply to the secondary button.
        /// </summary>
        public ControlAppearance SecondaryButtonAppearance
        {
            get => (ControlAppearance)GetValue(SecondaryButtonAppearanceProperty);
            set => SetValue(SecondaryButtonAppearanceProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="ControlAppearance"/> to apply to the close button.
        /// </summary>
        public ControlAppearance CloseButtonAppearance
        {
            get => (ControlAppearance)GetValue(CloseButtonAppearanceProperty);
            set => SetValue(CloseButtonAppearanceProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates which button on the dialog is the default action.
        /// </summary>
        public DialogButton DefaultButton
        {
            get => (DialogButton)GetValue(DefaultButtonProperty);
            set => SetValue(DefaultButtonProperty, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates the visibility of the footer buttons
        /// </summary>
        public bool IsFooterVisible
        {
            get => (bool)GetValue(IsFooterVisibleProperty);
            set => SetValue(IsFooterVisibleProperty, value);
        }

        /// <summary>
        /// Command triggered after clicking the button in the template.
        /// </summary>
        public IRelayCommand TemplateButtonCommand => (IRelayCommand)GetValue(TemplateButtonCommandProperty);

        #endregion

        public Dialog()
        {
            SetValue(TemplateButtonCommandProperty,
                new RelayCommand<DialogButton>(OnButtonClick));

            Loaded += static (sender, _) =>
            {
                var self = (Dialog)sender;
                self.OnLoaded();
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        /// <param name="contentPresenter"></param>
        public Dialog(ContentPresenter contentPresenter)
        {
            ContentPresenter = contentPresenter;

            SetValue(TemplateButtonCommandProperty,
                new RelayCommand<DialogButton>(OnButtonClick));

            Loaded += static (sender, _) =>
            {
                var self = (Dialog)sender;
                self.OnLoaded();
            };
        }

        protected readonly ContentPresenter ContentPresenter;
        protected TaskCompletionSource<DialogResult> Tcs;

        /// <summary>
        /// Shows the dialog
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="DialogResult"/></returns>
        /// <exception cref="TaskCanceledException"></exception>
        public async Task<DialogResult> ShowAsync(CancellationToken cancellationToken = default)
        {
            Tcs = new TaskCompletionSource<DialogResult>();
            CancellationTokenRegistration tokenRegistration = cancellationToken.Register(o => Tcs.TrySetCanceled((CancellationToken)o!), cancellationToken);

            try
            {
                ContentPresenter.Content = this;
                return await Tcs.Task;
            }
            finally
            {
                tokenRegistration.Dispose();
                ContentPresenter.Content = null;
                OnClosed();
            }
        }

        /// <summary>
        /// Hides the dialog with result
        /// </summary>
        public virtual void Hide(DialogResult result = DialogResult.None)
        {
            Tcs?.TrySetResult(result);
        }

        /// <summary>
        /// Occurs after Loading event
        /// </summary>
        protected virtual void OnLoaded()
        {
            if (VisualChildrenCount <= 0 || GetVisualChild(0) is not UIElement frameworkElement)
                return;

            ResizeToContentSize(frameworkElement);
            Focus();
        }

        /// <summary>
        /// Occurs after ContentPresenter.Content = null
        /// </summary>
        protected virtual void OnClosed()
        {

        }

        /// <summary>
        /// Sets <see cref="DialogWidth"/> and <see cref="DialogHeight"/>
        /// </summary>
        /// <param name="content"></param>
        protected virtual void ResizeToContentSize(UIElement content)
        {
            var paddingWidth = Padding.Left + Padding.Right;

            var marginHeight = DialogMargin.Bottom + DialogMargin.Top;
            var marginWidth = DialogMargin.Left + DialogMargin.Right;

            DialogWidth = content.DesiredSize.Width - marginWidth + paddingWidth;
            DialogHeight = content.DesiredSize.Height - marginHeight;

            while (true)
            {
                if (DialogWidth <= DialogMaxWidth && DialogHeight <= DialogMaxHeight)
                    return;

                if (DialogWidth > DialogMaxWidth)
                {
                    DialogWidth = DialogMaxWidth;
                    content.UpdateLayout();

                    DialogHeight = content.DesiredSize.Height;
                }

                if (DialogHeight > DialogMaxHeight)
                {
                    DialogHeight = DialogMaxHeight;
                    content.UpdateLayout();

                    DialogWidth = content.DesiredSize.Width;
                }
            }
        }

        /// <summary>
        /// Occurs after the <see cref="DialogButton"/> is clicked 
        /// </summary>
        /// <param name="button"></param>
        protected virtual void OnButtonClick(DialogButton button)
        {
            DialogResult result = button switch
            {
                DialogButton.Primary => DialogResult.Primary,
                DialogButton.Secondary => DialogResult.Secondary,
                _ => DialogResult.None
            };

            Hide(result);
        }
    }
}
