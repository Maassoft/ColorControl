namespace System.Net
{
    /// <summary>Provides a SecureOn password.</summary>
    [Serializable]
    public sealed class SecureOnPassword
    {
        private readonly byte[] _password;

        /// <summary>Initializes a new instance of <see cref="SecureOnPassword"/> with the given password.</summary>
        /// <param name="password">The password as <see cref="Byte"/> array.</param>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of the <see cref="T:System.Byte" /> array password is not 6.</exception>
        public SecureOnPassword(byte[] password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            if (password.Length != 6)
                throw new ArgumentException(Localization.ArgumentExceptionInvalidPasswordLength);
            _password = password;
        }

        /// <summary>Gets the buffer of the password.</summary>
        public byte[] GetPasswordBytes()
        {
            if (_password == null)
                return null;
            var buffer = new byte[_password.Length];
            Array.Copy(_password, buffer, 0);
            return buffer;
        }

        /// <summary>Initializes a new instance of <see cref="SecureOnPassword"/> with the given password.</summary>
        /// <param name="password">The password as <see cref="String"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null.</exception>
        /// <remarks>Uses <see cref="Text.Encoding.Default" /> as encoding.</remarks>
        public SecureOnPassword(string password)
            : this(password, Text.Encoding.Default)
        { }


        /// <summary>Initializes a new instance of <see cref="SecureOnPassword"/> with the given password.</summary>
        /// <param name="password">The password as <see cref="String"/>.</param>
        /// <param name="encoding">The <see cref="Text.Encoding"/> instance to use for the password.</param>
        /// <exception cref="ArgumentNullException"><paramref name="password"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="Byte"/> array wich is created using the password has more elements than 6.</exception>
        public SecureOnPassword(string password, Text.Encoding encoding)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            if (string.IsNullOrEmpty(password))
                _password = new byte[6];

            var bytes = encoding.GetBytes(password);
            if (bytes.Length > 6)
                throw new ArgumentException(Localization.ArgumentExceptionInvalidPasswordLength);

            _password = new byte[6];
            for (int i = 0; i < bytes.Length; i++)
                _password[i] = bytes[i];
            if (bytes.Length < 6)
            {
                for (int i = bytes.Length - 1; i < 6; i++)
                    _password[i] = 0x00;
            }
        }

        /// <summary>Converts the <see cref="SecureOnPassword"/> to dash notation.</summary>
        /// <returns>A <see cref="String"/> representing the <see cref="SecureOnPassword"/> as dash notation.</returns>
        public override string ToString() => ToString("X2");

        /// <summary>Converts the <see cref="SecureOnPassword"/> to dash notation.</summary>
        /// <returns>A <see cref="String"/> representing the <see cref="SecureOnPassword"/> as dash notation.</returns>
        private string ToString(string format)
        {
            var f = new string[6];
            for (int i = 0; i < f.Length; i++)
                f[i] = _password[i].ToString(format);
            return string.Join("-", f);
        }

        /// <summary>Converts the <see cref="SecureOnPassword"/> to dash notation.</summary>
        /// <returns>A <see cref="String"/> representing the <see cref="SecureOnPassword"/> as dash notation.</returns>
        public string ToString(IFormatProvider format)
        {
            var f = new string[6];
            for (int i = 0; i < f.Length; i++)
                f[i] = _password[i].ToString(format);
            return string.Join("-", f);
        }
    }
}
