using System;
using System.Data;
using System.Data.SqlClient;

namespace SistemManajemenKos
{
    internal class Program
    {
        private static string connectionString = "Data Source=LUTHFI\\LUTHFI;Integrated Security=True;";

        static void Main(string[] args)
        {
            Program pr = new Program();
            while (true)
            {
                try
                {
                    Console.WriteLine("Selamat datang di Sistem Manajemen Kos RIPKI");
                    Console.WriteLine("Pilih jenis autentikasi:");
                    Console.WriteLine("1. Windows Authentication");
                    Console.WriteLine("2. SQL Server Authentication");
                    Console.Write("Pilihan (1/2): ");
                    char authChoice = Convert.ToChar(Console.ReadLine());

                    using (SqlConnection conn = pr.GetSqlConnection(authChoice))
                    {
                        conn.Open();

                        Console.Write("Masukkan nama database yang akan digunakan: ");
                        string databaseName = Console.ReadLine().Trim();

                        if (string.IsNullOrEmpty(databaseName))
                        {
                            Console.WriteLine("Nama database tidak valid.");
                            continue;
                        }

                        string dynamicConnectionString = $"Data Source=LUTHFI\\LUTHFI;Initial Catalog={databaseName};Integrated Security=True;";

                        pr.CreateDatabase(conn, databaseName);
                        conn.ChangeDatabase(databaseName);
                        pr.CreateTablePenghuniKos(conn);
                        pr.CreateTableKamarKos(conn);
                        pr.CreateTablePembayaran(conn);

                        Console.Clear();

                        while (true)
                        {
                            Console.WriteLine("\nMenu Utama");
                            Console.WriteLine("1. Data Penghuni Kos");
                            Console.WriteLine("2. Data Kamar Kos");
                            Console.WriteLine("3. Data Pembayaran");
                            Console.WriteLine("4. Search Data");
                            Console.WriteLine("5. Keluar");
                            Console.WriteLine("\nMasukkan pilihan (1-5): ");
                            char menuChoice = Convert.ToChar(Console.ReadLine());

                            switch (menuChoice)
                            {
                                case '1':
                                    pr.ManagePenghuniKos(conn);
                                    break;
                                case '2':
                                    pr.ManageKamarKos(conn);
                                    break;
                                case '3':
                                    pr.ManagePembayaran(conn);
                                    break;
                                case '4':
                                    pr.SearchData(conn);
                                    break;
                                case '5':
                                    return;
                                default:
                                    Console.Clear();
                                    Console.WriteLine("\nPilihan tidak valid");
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Gagal terhubung ke database: {ex.Message}\n");
                    Console.ResetColor();
                }
            }
        }

        private SqlConnection GetSqlConnection(char authChoice)
        {
            string connectionStr = connectionString;
            if (authChoice == '2')
            {
                Console.Write("Masukkan User ID: ");
                string userId = Console.ReadLine();
                Console.Write("Masukkan Password: ");
                string password = Console.ReadLine();
                connectionStr = $"Data Source=LUTHFI\\LUTHFI;User ID={userId};Password={password};Integrated Security=False;";
            }

            return new SqlConnection(connectionStr);
        }

        private void CreateDatabase(SqlConnection conn, string databaseName)
        {
            try
            {

                string createDbQuery = $"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseName}') " +
                                       $"CREATE DATABASE {databaseName};";
                using (SqlCommand cmd = new SqlCommand(createDbQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Database '{databaseName}' berhasil dibuat atau sudah ada.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal membuat database '{databaseName}': {ex.Message}");
            }
        }

        private void CreateTablePenghuniKos(SqlConnection conn)
        {
            string createTableQuery = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PenghuniKos')
                                        CREATE TABLE PenghuniKos (
                                            ID INT PRIMARY KEY IDENTITY(1,1),
                                            Nama NVARCHAR(100) NOT NULL,
                                            Alamat NVARCHAR(200) NOT NULL
                                        );";

            using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Tabel 'PenghuniKos' berhasil dibuat atau sudah ada.");
            }
        }

        private void CreateTableKamarKos(SqlConnection conn)
        {
            string createTableQuery = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KamarKos')
                                        CREATE TABLE KamarKos (
                                            ID INT PRIMARY KEY IDENTITY(1,1),
                                            NomorKamar NVARCHAR(20) NOT NULL,
                                            Harga INT NOT NULL
                                        );";

            using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Tabel 'KamarKos' berhasil dibuat atau sudah ada.");
            }
        }

        private void CreateTablePembayaran(SqlConnection conn)
        {
            string createTableQuery = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Pembayaran')
                                        CREATE TABLE Pembayaran (
                                            ID INT PRIMARY KEY IDENTITY(1,1),
                                            IDPenghuni INT NOT NULL,
                                            Bulan NVARCHAR(20) NOT NULL,
                                            Tahun INT NOT NULL,
                                            Jumlah INT NOT NULL,
                                            FOREIGN KEY (IDPenghuni) REFERENCES PenghuniKos(ID)
                                        );";

            using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Tabel 'Pembayaran' berhasil dibuat atau sudah ada.");
            }
        }

        private void ManagePenghuniKos(SqlConnection conn)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("\nManajemen Data Penghuni Kos");
                Console.WriteLine("1. Tampilkan Data Penghuni");
                Console.WriteLine("2. Tambah Data Penghuni");
                Console.WriteLine("3. Hapus Data Penghuni");
                Console.WriteLine("4. Edit Data Penghuni");
                Console.WriteLine("5. Kembali ke Menu Utama");
                Console.Write("\nMasukkan pilihan (1-5): ");
                char choice = Convert.ToChar(Console.ReadLine());

                switch (choice)
                {
                    case '1':
                        DisplayPenghuniData(conn);
                        break;
                    case '2':
                        AddPenghuniData(conn);
                        break;
                    case '3':
                        DeletePenghuniData(conn);
                        break;
                    case '4':
                        EditPenghuni(conn);
                        break;
                    case '5':
                        Console.Clear();
                        return;
                    default:
                        Console.WriteLine("Pilihan tidak valid");
                        break;
                }
            }
        }

        private void DisplayPenghuniData(SqlConnection conn)
        {
            string query = "SELECT * FROM PenghuniKos;";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["ID"]}, Nama: {reader["Nama"]}, Alamat: {reader["Alamat"]}");
                }
                reader.Close();
            }
        }

        private void AddPenghuniData(SqlConnection conn)
        {
            Console.WriteLine("Masukkan data penghuni baru:");
            Console.Write("Nama: ");
            string nama = Console.ReadLine();
            Console.Write("Alamat: ");
            string alamat = Console.ReadLine();

            if (IsPenghuniNameExists(conn, nama))
            {
                Console.WriteLine("Nama penghuni sudah ada dalam database. Masukkan nama penghuni yang berbeda.");
                return;
            }

            string insertQuery = "INSERT INTO PenghuniKos (Nama, Alamat) OUTPUT INSERTED.ID VALUES (@Nama, @Alamat);";
            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Nama", nama);
                cmd.Parameters.AddWithValue("@Alamat", alamat);

                int newPenghuniId = (int)cmd.ExecuteScalar();

                Console.WriteLine($"Data penghuni dengan ID {newPenghuniId} berhasil ditambahkan.");
            }
        }

        private bool IsPenghuniNameExists(SqlConnection conn, string nama)
        {
            string query = "SELECT COUNT(*) FROM PenghuniKos WHERE Nama = @Nama;";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Nama", nama);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void DeletePenghuniData(SqlConnection conn)
        {
            Console.Write("Masukkan ID Penghuni yang ingin dihapus: ");
            int id = Convert.ToInt32(Console.ReadLine());

            if (!IsPenghuniIdExists(conn, id))
            {
                Console.WriteLine("ID Penghuni tidak ditemukan dalam database.");
                return;
            }

            Console.Write("Apakah Anda yakin ingin menghapus data penghuni ini? (ya/tidak): ");
            string confirmation = Console.ReadLine().ToLower();

            if (confirmation == "ya")
            {
                string deletePembayaranQuery = "DELETE FROM Pembayaran WHERE IDPenghuni = @ID;";
                using (SqlCommand cmd = new SqlCommand(deletePembayaranQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }

                string deletePenghuniQuery = "DELETE FROM PenghuniKos WHERE ID = @ID;";
                using (SqlCommand cmd = new SqlCommand(deletePenghuniQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} data penghuni berhasil dihapus.");
                }
            }
            else if (confirmation == "tidak")
            {
                Console.WriteLine("Operasi penghapusan data dibatalkan.");
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
        }

        private bool IsPenghuniIdExists(SqlConnection conn, int id)
        {
            string query = "SELECT COUNT(*) FROM PenghuniKos WHERE ID = @ID;";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void EditPenghuni(SqlConnection conn)
        {
            Console.Write("Masukkan ID Penghuni yang ingin diedit: ");
            int idPenghuni = Convert.ToInt32(Console.ReadLine());

            Console.Write("Masukkan nama penghuni baru: ");
            string nama = Console.ReadLine();
            Console.Write("Masukkan alamat penghuni baru: ");
            string alamat = Console.ReadLine();

            string updateQuery = "UPDATE PenghuniKos SET Nama = @Nama, Alamat = @Alamat WHERE ID = @ID;";
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Nama", nama);
                cmd.Parameters.AddWithValue("@Alamat", alamat);
                cmd.Parameters.AddWithValue("@ID", idPenghuni);

                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} data penghuni berhasil diubah.");
            }
        }

        private void ManageKamarKos(SqlConnection conn)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("\nManajemen Data Kamar Kos");
                Console.WriteLine("1. Tampilkan Data Kamar");
                Console.WriteLine("2. Tambah Data Kamar");
                Console.WriteLine("3. Hapus Data Kamar");
                Console.WriteLine("4. Edit Data Kamar");
                Console.WriteLine("5. Kembali ke Menu Utama");
                Console.Write("\nMasukkan pilihan (1-5): ");
                char choice = Convert.ToChar(Console.ReadLine());

                switch (choice)
                {
                    case '1':
                        DisplayKamarData(conn);
                        break;
                    case '2':
                        AddKamarData(conn);
                        break;
                    case '3':
                        DeleteKamarData(conn);
                        break;
                    case '4':
                        EditKamar(conn);
                        break;
                    case '5':
                        Console.Clear();
                        return;
                    default:
                        Console.WriteLine("Pilihan tidak valid");
                        break;
                }
            }
        }

        private void DisplayKamarData(SqlConnection conn)
        {
            string query = "SELECT * FROM KamarKos;";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["ID"]}, Nomor Kamar: {reader["NomorKamar"]}, Harga: {reader["Harga"]}");
                }
                reader.Close();
            }
        }

        private void AddKamarData(SqlConnection conn)
        {
            Console.WriteLine("Masukkan data kamar baru:");
            Console.Write("Nomor Kamar: ");
            string nomorKamar = Console.ReadLine();
            Console.Write("Harga: ");
            int harga = Convert.ToInt32(Console.ReadLine());

            string insertQuery = "INSERT INTO KamarKos (NomorKamar, Harga) VALUES (@NomorKamar, @Harga);";
            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@NomorKamar", nomorKamar);
                cmd.Parameters.AddWithValue("@Harga", harga);

                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} data kamar berhasil ditambahkan.");
            }
        }

        private void DeleteKamarData(SqlConnection conn) 
        {
            Console.Write("Masukkan ID Kamar yang ingin dihapus: ");
            int id = Convert.ToInt32(Console.ReadLine());

            if (!IsKamarIdExists(conn, id))
            {
                Console.WriteLine("ID Kamar tidak ditemukan dalam database.");
                return;
            }

            Console.Write("Apakah Anda yakin ingin menghapus data kamar ini? (ya/tidak): ");
            string confirmation = Console.ReadLine().ToLower();

            if (confirmation == "ya")
            {
                string deleteKamarQuery = "DELETE FROM KamarKos WHERE ID = @ID;";
                using (SqlCommand cmd = new SqlCommand(deleteKamarQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} data kamar berhasil dihapus.");
                }
            }
            else if (confirmation == "tidak")
            {
                Console.WriteLine("Operasi penghapusan data dibatalkan.");
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
        }

        private bool IsKamarIdExists(SqlConnection conn, int id)
        {
            string query = "SELECT COUNT(*) FROM KamarKos WHERE ID = @ID;";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void EditKamar(SqlConnection conn)
        {
            Console.Write("Masukkan ID Kamar yang ingin diedit: ");
            int idKamar = Convert.ToInt32(Console.ReadLine());

            Console.Write("Masukkan nomor kamar baru: ");
            string nomorKamar = Console.ReadLine();
            Console.Write("Masukkan harga kamar baru: ");
            int harga = Convert.ToInt32(Console.ReadLine());

            string updateQuery = "UPDATE KamarKos SET NomorKamar = @NomorKamar, Harga = @Harga WHERE ID = @ID;";
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@NomorKamar", nomorKamar);
                cmd.Parameters.AddWithValue("@Harga", harga);
                cmd.Parameters.AddWithValue("@ID", idKamar);

                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} data kamar berhasil diubah.");
            }
        }

        private void ManagePembayaran(SqlConnection conn)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("\nManajemen Data Pembayaran");
                Console.WriteLine("1. Tampilkan Data Pembayaran");
                Console.WriteLine("2. Tambah Data Pembayaran");
                Console.WriteLine("3. Hapus Data Pembayaran");
                Console.WriteLine("4. Edit Data Pembayaran");
                Console.WriteLine("5. Kembali ke Menu Utama");
                Console.Write("\nMasukkan pilihan (1-5): ");
                char choice = Convert.ToChar(Console.ReadLine());

                switch (choice)
                {
                    case '1':
                        DisplayPembayaranData(conn);
                        break;
                    case '2':
                        AddPembayaranData(conn);
                        break;
                    case '3':
                        DeletePembayaranData(conn);
                        break;
                    case '4':
                        EditPembayaran(conn);
                        break;
                    case '5':
                        Console.Clear();
                        return;
                    default:
                        Console.WriteLine("Pilihan tidak valid");
                        break;
                }
            }
        }

        private void DisplayPembayaranData(SqlConnection conn)
        {
            string query = "SELECT * FROM Pembayaran;";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["ID"]}, ID Penghuni: {reader["IDPenghuni"]}, Bulan: {reader["Bulan"]}, Tahun: {reader["Tahun"]}, Jumlah: {reader["Jumlah"]}");
                }
                reader.Close();
            }
        }

        private void AddPembayaranData(SqlConnection conn)
        {
            Console.WriteLine("Masukkan data pembayaran baru:");
            Console.Write("ID Penghuni: ");
            int idPenghuni = Convert.ToInt32(Console.ReadLine());
            Console.Write("Bulan: ");
            string bulan = Console.ReadLine();
            Console.Write("Tahun: ");
            int tahun = Convert.ToInt32(Console.ReadLine());
            Console.Write("Jumlah: ");
            int jumlah = Convert.ToInt32(Console.ReadLine());

            string insertQuery = "INSERT INTO Pembayaran (IDPenghuni, Bulan, Tahun, Jumlah) VALUES (@IDPenghuni, @Bulan, @Tahun, @Jumlah);";
            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@IDPenghuni", idPenghuni);
                cmd.Parameters.AddWithValue("@Bulan", bulan);
                cmd.Parameters.AddWithValue("@Tahun", tahun);
                cmd.Parameters.AddWithValue("@Jumlah", jumlah);

                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} data pembayaran berhasil ditambahkan.");
            }
        }

        private void DeletePembayaranData(SqlConnection conn)
        {
            Console.Write("Masukkan ID Pembayaran yang ingin dihapus: ");
            int id = Convert.ToInt32(Console.ReadLine());

            if (!IsPembayaranIdExists(conn, id))
            {
                Console.WriteLine("ID Pembayaran tidak ditemukan dalam database.");
                return;
            }

            Console.Write("Apakah Anda yakin ingin menghapus data pembayaran ini? (ya/tidak): ");
            string confirmation = Console.ReadLine().ToLower();

            if (confirmation == "ya")
            {
                string deletePembayaranQuery = "DELETE FROM Pembayaran WHERE ID = @ID;";
                using (SqlCommand cmd = new SqlCommand(deletePembayaranQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"{rowsAffected} data pembayaran berhasil dihapus.");
                }
            }
            else if (confirmation == "tidak")
            {
                Console.WriteLine("Operasi penghapusan data dibatalkan.");
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
        }

        private bool IsPembayaranIdExists(SqlConnection conn, int id)
        {
            string query = "SELECT COUNT(*) FROM Pembayaran WHERE ID = @ID;";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ID", id);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void EditPembayaran(SqlConnection conn)
        {
            Console.Write("Masukkan ID Pembayaran yang ingin diedit: ");
            int idPembayaran = Convert.ToInt32(Console.ReadLine());

            Console.Write("Masukkan ID penghuni baru: ");
            int idPenghuni = Convert.ToInt32(Console.ReadLine());
            Console.Write("Masukkan bulan baru: ");
            string bulan = Console.ReadLine();
            Console.Write("Masukkan tahun baru: ");
            int tahun = Convert.ToInt32(Console.ReadLine());
            Console.Write("Masukkan jumlah baru: ");
            int jumlah = Convert.ToInt32(Console.ReadLine());

            string updateQuery = "UPDATE Pembayaran SET IDPenghuni = @IDPenghuni, Bulan = @Bulan, Tahun = @Tahun, Jumlah = @Jumlah WHERE ID = @ID;";
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@IDPenghuni", idPenghuni);
                cmd.Parameters.AddWithValue("@Bulan", bulan);
                cmd.Parameters.AddWithValue("@Tahun", tahun);
                cmd.Parameters.AddWithValue("@Jumlah", jumlah);
                cmd.Parameters.AddWithValue("@ID", idPembayaran);

                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} data pembayaran berhasil diubah.");
            }
        }

        private void SearchData(SqlConnection conn)
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("\nMenu Pencarian Data");
                Console.WriteLine("1. Cari Penghuni Kos");
                Console.WriteLine("2. Cari Kamar Kos");
                Console.WriteLine("3. Cari Pembayaran");
                Console.WriteLine("4. Kembali ke Menu Utama");
                Console.Write("\nMasukkan pilihan (1-4): ");
                char choice = Convert.ToChar(Console.ReadLine());

                switch (choice)
                {
                    case '1':
                        SearchPenghuni(conn);
                        break;
                    case '2':
                        SearchKamar(conn);
                        break;
                    case '3':
                        SearchPembayaran(conn);
                        break;
                    case '4':
                        Console.Clear();
                        return;
                    default:
                        Console.WriteLine("Pilihan tidak valid");
                        break;
                }
            }
        }

        private void SearchPenghuni(SqlConnection conn)
        {
            Console.Write("Masukkan nama penghuni yang ingin dicari: ");
            string nama = Console.ReadLine();

            string query = $"SELECT * FROM PenghuniKos WHERE Nama LIKE '%{nama}%';";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["ID"]}, Nama: {reader["Nama"]}, Alamat: {reader["Alamat"]}");
                }
                reader.Close();
            }
        }

        private void SearchKamar(SqlConnection conn)
        {
            Console.Write("Masukkan nomor kamar yang ingin dicari: ");
            string nomorKamar = Console.ReadLine();

            string query = $"SELECT * FROM KamarKos WHERE NomorKamar LIKE '%{nomorKamar}%';";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["ID"]}, Nomor Kamar: {reader["NomorKamar"]}, Harga: {reader["Harga"]}");
                }
                reader.Close();
            }
        }

        private void SearchPembayaran(SqlConnection conn)
        {
            Console.Write("Masukkan bulan yang ingin dicari: ");
            string bulan = Console.ReadLine();
            Console.Write("Masukkan tahun yang ingin dicari: ");
            int tahun = Convert.ToInt32(Console.ReadLine());

            string query = $"SELECT * FROM Pembayaran WHERE Bulan = '{bulan}' AND Tahun = {tahun};";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["ID"]}, ID Penghuni: {reader["IDPenghuni"]}, Bulan: {reader["Bulan"]}, Tahun: {reader["Tahun"]}, Jumlah: {reader["Jumlah"]}");
                }
                reader.Close();
            }
        }
    }
}
