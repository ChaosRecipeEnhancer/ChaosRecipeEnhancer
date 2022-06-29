using System;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.App.Services;
using Winook;

namespace ChaosRecipeEnhancer.App.Native
{
    public class MouseManager : IDisposable
    {
        #region Fields

        private SettingsService _settingsService;
        private MouseHook _mouseHook;
        private bool _disposed = false;
        private int _x;
        private int _y;

        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseManager" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="settingsService">The settings service.</param>
        public MouseManager(int processId, SettingsService settingsService)
        {
            _settingsService = settingsService;
            _mouseHook = new MouseHook(processId, MouseMessageTypes.All);

            _mouseHook.LeftButtonUp += MouseHook_LeftButtonUp;
            _mouseHook.MouseMove += MouseHook_MouseMove;
            _mouseHook.InstallAsync();
        }

        #endregion
        
        #region Events
        
        /// <summary>
        /// Occurs when [mouse move].
        /// </summary>
        public event EventHandler<MouseMessageEventArgs> MouseMove;

        /// <summary>
        /// Occurs when [mouse left button up].
        /// </summary>
        public event EventHandler<MouseMessageEventArgs> MouseLeftButtonUp;
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the x.
        /// </summary>
        public int X => _x;

        /// <summary>
        /// Gets the y.
        /// </summary>
        public int Y => _y;

        #endregion
        
        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose() => Dispose(true);
        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        _mouseHook.MouseMove -= MouseHook_MouseMove;
                        _mouseHook.LeftButtonUp -= MouseHook_LeftButtonUp;
                        _mouseHook.Dispose();
                    }
                    catch
                    {
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the MouseHook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseMessageEventArgs"/> instance containing the event data.</param>
        private void MouseHook_MouseMove(object sender, MouseMessageEventArgs e)
        {
            _x = e.X;
            _y = e.Y;

            MouseMove?.Invoke(this, e);
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the MouseHookService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MouseHook_LeftButtonUp(object sender, MouseMessageEventArgs e)
        {
            MouseLeftButtonUp?.Invoke(this, e);
        }

        #endregion
    }
}