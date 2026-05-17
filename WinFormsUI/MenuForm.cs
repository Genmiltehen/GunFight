namespace WinFormsUI
{
    public partial class MenuForm : Form
    {
        private readonly ComboBox cb1;
        private readonly ComboBox cb2;
        private readonly Button btnOk;

        public string PlayerA { get; private set; } = "";
        public string PlayerB { get; private set; } = "";
        private readonly string[] vars;

        public MenuForm(string[] variants)
        {
            InitializeComponent();
            vars = variants;

            Text = "Выберите игроков";
            Size = new Size(300, 150);
            StartPosition = FormStartPosition.CenterParent;

            var lbl1 = new Label { Text = "Игрок А:", Location = new(10, 13), AutoSize = true };
            cb1 = new ComboBox { Location = new(100, 10), Width = 170 };

            var lbl2 = new Label { Text = "Игрок Б:", Location = new(10, 43), AutoSize = true };
            cb2 = new ComboBox { Location = new(100, 40), Width = 170 };

            btnOk = new Button { Text = "OK", Location = new(10, 70), DialogResult = DialogResult.OK };

            cb1.Items.AddRange(vars);
            cb2.Items.AddRange(vars);

            Controls.Add(lbl1);
            Controls.Add(cb1);
            Controls.Add(lbl2);
            Controls.Add(cb2);
            Controls.Add(btnOk);

            AcceptButton = btnOk;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                PlayerA = cb1.SelectedItem?.ToString() ?? vars[0];
                PlayerB = cb2.SelectedItem?.ToString() ?? vars[0];
            }
            base.OnFormClosing(e);
        }
    }
}
