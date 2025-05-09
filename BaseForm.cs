#nullable enable
using System;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace RockTalk
{
    public partial class BaseForm : Form
    {
        protected readonly SupabaseService _supabaseService;
        protected readonly ILogger<BaseForm> _logger;
        protected string? _currentUserId; // Nullable per evitare CS8625

        public BaseForm(SupabaseService supabaseService, ILogger<BaseForm> logger)
        {
            InitializeComponent();
            _supabaseService = supabaseService ?? throw new ArgumentNullException(nameof(supabaseService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserId = null; // Inizializzazione esplicita per evitare CS8625
        }

        protected virtual async Task InitializeFormAsync()
        {
            try
            {
                await _supabaseService.InitializeAsync();
                _logger.LogInformation("Supabase initialized for form: {FormName}", this.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Supabase for form: {FormName}", this.GetType().Name);
                MessageBox.Show("Errore durante l'inizializzazione del servizio.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void OnFormClosingOverride(FormClosingEventArgs e)
        {
            // Sovrascrivibile dalle form derivate
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            OnFormClosingOverride(e);
        }

        protected void SetCurrentUserId(string? userId)
        {
            _currentUserId = userId;
            _logger.LogInformation("Current user ID set to: {UserId}", userId ?? "null");
        }

        protected void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "BaseForm";
            this.Text = "Base Form";
            this.ResumeLayout(false);
        }
    }
}
