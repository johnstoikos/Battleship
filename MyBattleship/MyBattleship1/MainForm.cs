using Battleship;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MyBattleship;

public partial class MainForm : Form
{
    const int ROWS = 11;
    const int COLS = 10;
    readonly char[] ROW_LETTERS = "ABCDEFGHIJK".ToCharArray();

    DataGridView grdPlayer;
    DataGridView grdEnemy;
    Label lblStatus, lblEnemyTitle, lblEnemyList, lblPlayerTitle, lblPlayerList;
    Button btnNew;

    readonly Color HitColor = Color.FromArgb(230, 60, 60);
    readonly Color MissColorEnemy = Color.FromArgb(225, 230, 235);
    readonly Color MissColorPlayer = Color.FromArgb(210, 235, 210);
    readonly Color RowColBack = Color.FromArgb(245, 245, 245);
    readonly string MissGlyph = "·";

    Random rnd = new Random();
    string[] SHIP_NAMES = { "Αεροπλανοφόρο", "Αντιτορπιλικό", "Πολεμικό", "Υποβρύχιο" };
    int[] SHIP_SIZES = { 5, 4, 3, 2 };

    Board playerBoard = null!;
    Board enemyBoard = null!;

    int moves = 0;
    DateTime startTime;
    HashSet<string> aiTried = new HashSet<string>();
    HashSet<string> enemySunk = new HashSet<string>();
    HashSet<string> playerSunk = new HashSet<string>();

    public MainForm()
    {
        InitializeComponent();

        Text = "Ναυμαχία (Student Edition)";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        grdPlayer = MakeGrid();
        grdEnemy = MakeGrid();
        lblStatus = new Label { AutoSize = true, Font = new Font("Segoe UI", 10f) };
        btnNew = new Button { Text = "Νέα Παρτίδα", Size = new Size(120, 32) };

        lblEnemyTitle = new Label { Text = "Βυθίστηκαν (Αντίπαλος):", AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
        lblEnemyList = new Label { Text = "—", AutoSize = true };
        lblPlayerTitle = new Label { Text = "Βυθίστηκαν (Παίκτης):", AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
        lblPlayerList = new Label { Text = "—", AutoSize = true };

        Controls.Add(grdPlayer);
        Controls.Add(grdEnemy);
        Controls.Add(lblStatus);
        Controls.Add(btnNew);
        Controls.Add(lblEnemyTitle);
        Controls.Add(lblEnemyList);
        Controls.Add(lblPlayerTitle);
        Controls.Add(lblPlayerList);

        grdPlayer.Location = new Point(20, 20);
        grdEnemy.Location = new Point(grdPlayer.Right + 30, 20);

        lblStatus.Location = new Point(20, grdPlayer.Bottom + 10);
        btnNew.Location = new Point(20, lblStatus.Bottom + 8);

        lblEnemyTitle.Location = new Point(grdEnemy.Left, grdEnemy.Bottom + 10);
        lblEnemyList.Location = new Point(lblEnemyTitle.Left, lblEnemyTitle.Bottom + 4);
        lblPlayerTitle.Location = new Point(grdEnemy.Left, lblEnemyList.Bottom + 10);
        lblPlayerList.Location = new Point(lblPlayerTitle.Left, lblPlayerTitle.Bottom + 4);

        grdEnemy.CellClick += GrdEnemy_CellClick;
        btnNew.Click += (s, e) => { StartGame(); CenterLayout(); };

        SetupGrid(grdPlayer);
        SetupGrid(grdEnemy);

        StartGame();
        CenterLayout();

        ClientSize = new Size(Math.Max(grdEnemy.Right + 20, lblPlayerList.Right + 20),
                              Math.Max(btnNew.Bottom, lblPlayerList.Bottom) + 20);
        CenterLayout();
    }

    DataGridView MakeGrid()
    {
        var g = new DataGridView
        {
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeColumns = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            RowHeadersVisible = false,
            ColumnHeadersVisible = true,
            ScrollBars = ScrollBars.None,
            BackgroundColor = Color.White,
            GridColor = Color.Gainsboro,
            CellBorderStyle = DataGridViewCellBorderStyle.Single,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
        };
        g.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        g.DefaultCellStyle.Font = new Font("Segoe UI", 10f);
        g.DefaultCellStyle.Padding = new Padding(0, 2, 0, 0);
        g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        return g;
    }

    void SetupGrid(DataGridView g)
    {
        g.Columns.Clear();
        g.Rows.Clear();

        int cell = 34;

        var rowCol = new DataGridViewTextBoxColumn();
        rowCol.HeaderText = "";
        rowCol.Width = cell;
        rowCol.ReadOnly = true;
        rowCol.DefaultCellStyle.BackColor = RowColBack;
        rowCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        g.Columns.Add(rowCol);

        for (int c = 0; c < COLS; c++)
        {
            var col = new DataGridViewTextBoxColumn();
            col.HeaderText = (c + 1).ToString();
            col.Width = cell;
            g.Columns.Add(col);
        }

        for (int r = 0; r < ROWS; r++)
        {
            g.Rows.Add();
            g.Rows[r].Height = cell;
            g[0, r].Value = ROW_LETTERS[r].ToString();
        }

        int w = 0; for (int i = 0; i < g.Columns.Count; i++) w += g.Columns[i].Width;
        int h = g.ColumnHeadersHeight; for (int i = 0; i < g.Rows.Count; i++) h += g.Rows[i].Height;
        g.Size = new Size(w + 2, h + 2);

        g.ClearSelection();
    }

    void CenterLayout()
    {
        int totalWidth = grdPlayer.Width + 30 + grdEnemy.Width;
        int left = Math.Max(20, (ClientSize.Width - totalWidth) / 2);
        grdPlayer.Left = left;
        grdEnemy.Left = grdPlayer.Right + 30;

        lblStatus.Top = grdPlayer.Bottom + 10;
        btnNew.Top = lblStatus.Bottom + 8;

        lblEnemyTitle.Top = grdEnemy.Bottom + 10;
        lblEnemyList.Top = lblEnemyTitle.Bottom + 4;
        lblPlayerTitle.Top = lblEnemyList.Bottom + 10;
        lblPlayerList.Top = lblPlayerTitle.Bottom + 4;
    }

    void StartGame()
    {
        playerBoard = new Board(ROWS, COLS);
        enemyBoard = new Board(ROWS, COLS);

        for (int i = 0; i < SHIP_NAMES.Length; i++)
        {
            playerBoard.PlaceShip(new Ship(SHIP_NAMES[i], SHIP_SIZES[i]));
            enemyBoard.PlaceShip(new Ship(SHIP_NAMES[i], SHIP_SIZES[i]));
        }

        for (int r = 0; r < ROWS; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                Ship? s = playerBoard.GetShipAt(r, c);
                grdPlayer[c + 1, r].Value = s == null ? "" : s.Code;
                grdPlayer[c + 1, r].Style.BackColor = Color.White;
                grdPlayer[c + 1, r].Style.ForeColor = Color.Black;

                grdEnemy[c + 1, r].Value = "";
                grdEnemy[c + 1, r].Style.BackColor = Color.White;
                grdEnemy[c + 1, r].Style.ForeColor = Color.Black;
            }
        }

        moves = 0;
        startTime = DateTime.Now;
        aiTried.Clear();
        enemySunk.Clear();
        playerSunk.Clear();
        UpdateSunkLabels();

        lblStatus.Text = "Η σειρά σου!";
        grdEnemy.Enabled = true;
    }

    void GrdEnemy_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex <= 0) return;
        if (!grdEnemy.Enabled) return;

        if (grdEnemy[e.ColumnIndex, e.RowIndex].Value != null &&
            grdEnemy[e.ColumnIndex, e.RowIndex].Value.ToString() != "") return;

        moves++;

        int r = e.RowIndex;
        int c = e.ColumnIndex - 1;

        Ship? ship;
        bool hit = enemyBoard.Shoot(r, c, out ship);
        if (hit)
        {
            grdEnemy[c + 1, r].Value = "X";
            grdEnemy[c + 1, r].Style.BackColor = HitColor;
            grdEnemy[c + 1, r].Style.ForeColor = Color.White;

            if (ship != null && ship.IsSunk())
            {
                lblStatus.Text = "Βύθισες το " + ship.Name + "!";
                enemySunk.Add(ship.Name);
                UpdateSunkLabels();
            }
            else
            {
                lblStatus.Text = "Εύστοχο! Ξαναπαίζεις.";
            }

            if (enemyBoard.AllSunk())
            {
                grdEnemy.Enabled = false;
                lblStatus.Text = "Κέρδισες! Προσπάθειες: " + moves +
                                 ", Χρόνος: " + (DateTime.Now - startTime).TotalSeconds.ToString("F1") + "s";
            }
            return;
        }
        else
        {
            grdEnemy[c + 1, r].Value = MissGlyph;
            grdEnemy[c + 1, r].Style.BackColor = MissColorEnemy;
            grdEnemy[c + 1, r].Style.ForeColor = Color.DimGray;
            lblStatus.Text = "Άστοχο! Σειρά του υπολογιστή.";
        }

        if (enemyBoard.AllSunk())
        {
            grdEnemy.Enabled = false;
            lblStatus.Text = "Κέρδισες! Προσπάθειες: " + moves +
                             ", Χρόνος: " + (DateTime.Now - startTime).TotalSeconds.ToString("F1") + "s";
            return;
        }

        AiTurnLoop();
    }

    void AiTurnLoop()
    {
        bool hit;
        do
        {
            this.Refresh();
            System.Threading.Thread.Sleep(300);
            hit = AiTurnOnce();
            if (playerBoard.AllSunk()) return;
        }
        while (hit);

        lblStatus.Text = "Η σειρά σου!";
    }

    bool AiTurnOnce()
    {
        int r, c;
        string key;
        do
        {
            r = rnd.Next(ROWS);
            c = rnd.Next(COLS);
            key = r + "," + c;
        }
        while (aiTried.Contains(key));
        aiTried.Add(key);

        Ship? ship;
        bool hit = playerBoard.Shoot(r, c, out ship);
        if (hit)
        {
            grdPlayer[c + 1, r].Value = "X";
            grdPlayer[c + 1, r].Style.BackColor = HitColor;
            grdPlayer[c + 1, r].Style.ForeColor = Color.White;

            if (ship != null && ship.IsSunk())
            {
                lblStatus.Text = "Ο υπολογιστής βύθισε το " + ship.Name + "! Παίζει ξανά.";
                playerSunk.Add(ship.Name);
                UpdateSunkLabels();
            }
            else
            {
                lblStatus.Text = "Ο υπολογιστής σε χτύπησε! Παίζει ξανά.";
            }
        }
        else
        {
            grdPlayer[c + 1, r].Value = MissGlyph;
            grdPlayer[c + 1, r].Style.BackColor = MissColorPlayer;
            grdPlayer[c + 1, r].Style.ForeColor = Color.DarkGreen;
            lblStatus.Text = "Ο υπολογιστής αστόχησε!";
        }

        if (playerBoard.AllSunk())
        {
            grdEnemy.Enabled = false;
            lblStatus.Text = "Έχασες! Προσπάθειες: " + moves +
                             ", Χρόνος: " + (DateTime.Now - startTime).TotalSeconds.ToString("F1") + "s";
        }

        return hit;
    }

    void UpdateSunkLabels()
    {
        var eList = new List<string>(enemySunk); eList.Sort();
        var pList = new List<string>(playerSunk); pList.Sort();

        lblEnemyList.Text = eList.Count > 0 ? string.Join(", ", eList.ToArray()) : "—";
        lblPlayerList.Text = pList.Count > 0 ? string.Join(", ", pList.ToArray()) : "—";
    }
}
