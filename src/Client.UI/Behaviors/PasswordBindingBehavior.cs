using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Client.UI.Behaviors
{
    /// <summary>
    /// PasswordBindingBehavior クラス
    /// </summary>
    public class PasswordBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// PasswordProperty
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(PasswordBindingBehavior),
                new PropertyMetadata(string.Empty, PasswordPropertyChanged));

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password
        {
            get { return (string)this.GetValue(PasswordProperty); }
            set { this.SetValue(PasswordProperty, value); }
        }

        /// <summary>
        /// OnAttached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PasswordChanged += this.PasswordBox_PasswordChanged;
        }

        /// <summary>
        /// OnDetaching
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.PasswordChanged -= this.PasswordBox_PasswordChanged;
            base.OnDetaching();
        }

        /// <summary>
        /// PasswordPropertyChanged
        /// </summary>
        /// <param name="d">DependencyObject</param>
        /// <param name="e">DependencyPropertyChangedEventArgs</param>
        private static void PasswordPropertyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PasswordBindingBehavior behavior)
                || !(behavior.AssociatedObject is PasswordBox passwordBox)
                || !(e.NewValue is string newPassword))
            {
                return;
            }

            var oldPassword = e.OldValue as string;
            if (newPassword.Equals(oldPassword))
            {
                return;
            }

            if (passwordBox.Password == newPassword)
            {
                return;
            }

            passwordBox.Password = newPassword;
        }

        /// <summary>
        /// PasswordBox_PasswordChanged
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Password = this.AssociatedObject.Password;
        }
    }
}
