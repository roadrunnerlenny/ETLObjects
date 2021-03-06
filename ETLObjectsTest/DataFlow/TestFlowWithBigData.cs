﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ETLObjects;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace ETLObjectsTest
{
    public partial class ETLObjectsTest
    {

        public class Datensatz
        {
            public int Key;
            public int F1;
            public int F2;
            public int F3;
            public int F4;
            public int F5;
            public int F6;
            public int F7;
            public int F8;
            public int F9;
            public int F10;
            public int F1_calc;
            public int F2_calc;
            public int F3_calc;
            public int F4_calc;
            public int F5_calc;
            public int F6_calc;
            public int F7_calc;
            public int F8_calc;
            public int F9_calc;
            public int F10_calc;
        }

        public class ReaderAdapter
        {
            public static Datensatz Read(IDataRecord record)
            {
                var Datensatz = new Datensatz();
                Datensatz.Key = record.GetInt32(0);
                Datensatz.F1 = record.GetInt32(1);
                Datensatz.F2 = record.GetInt32(2);
                Datensatz.F3 = record.GetInt32(3);
                Datensatz.F4 = record.GetInt32(4);
                Datensatz.F5 = record.GetInt32(5);
                Datensatz.F6 = record.GetInt32(6);
                Datensatz.F7 = record.GetInt32(7);
                Datensatz.F8 = record.GetInt32(8);
                Datensatz.F9 = record.GetInt32(9);
                Datensatz.F10 = record.GetInt32(10);
                return Datensatz;
            }
        }

        private static int FieldCount = 21;
        public class WriterAdapter
        {
            public static object[] Fill(Datensatz Datensatz)
            {
                object[] record = new object[FieldCount];
                record[0] = Datensatz.Key;
                record[1] = Datensatz.F1;
                record[2] = Datensatz.F1_calc;
                record[3] = Datensatz.F2;
                record[4] = Datensatz.F2_calc;
                record[5] = Datensatz.F3;
                record[6] = Datensatz.F3_calc;
                record[7] = Datensatz.F4;
                record[8] = Datensatz.F4_calc;
                record[9] = Datensatz.F5;
                record[10] = Datensatz.F5_calc;
                record[11] = Datensatz.F6;
                record[12] = Datensatz.F6_calc;
                record[13] = Datensatz.F7;
                record[14] = Datensatz.F7_calc;
                record[15] = Datensatz.F8;
                record[16] = Datensatz.F8_calc;
                record[17] = Datensatz.F9;
                record[18] = Datensatz.F9_calc;
                record[19] = Datensatz.F10;
                record[20] = Datensatz.F10_calc;
                return record;
            }
        }

        public Datensatz RowTransformationDB(Datensatz row)
        {
 
            row.F1_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F1).IntervalScore;
            row.F2_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F2).IntervalScore;
            row.F3_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F3).IntervalScore;
            row.F4_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F4).IntervalScore;
            row.F5_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F5).IntervalScore;
            row.F6_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F6).IntervalScore;
            row.F7_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F7).IntervalScore;
            row.F8_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F8).IntervalScore;
            row.F9_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F9).IntervalScore;
            row.F10_calc = IntervalPointSearch.IntervalPointLinearSearch(MetrischeSkala, (decimal)row.F10).IntervalScore;

            return row;
        }

        List<IntervalPointMetric> MetrischeSkala = new List<IntervalPointMetric>();

        [TestMethod]
        public void TestDataflow_Massendaten()
        {
            using (TestDb)
            {
                int SkalaGrenze = 10000;

                for (int i = SkalaGrenze * -1; i < SkalaGrenze; i = i + 50)
                {
                    MetrischeSkala.Add(new IntervalPointMetric(i, i));
                }


                int Anzahl_je_Faktor = 10000;
                int Anzahl_Faktoren = 10;

                string TempObjectNameName = "test.tmp";
                new DropTableTask(TestDb.getNewSqlConnection()).Execute(TempObjectNameName);

                string QuellSchemaName = "test";
                string QuellTabelle = "source";
                string QuellObjekt = $"[{QuellSchemaName}].[{QuellTabelle}]";

                new DropAndCreateTableTask(TestDb.getNewSqlConnection()).Execute(QuellSchemaName, QuellTabelle, new List<TableColumn>() {
                new TableColumn("Key", SqlDbType.Int, false, true, true),
                new TableColumn("F1", SqlDbType.Int, true),
                new TableColumn("F2", SqlDbType.Int, true),
                new TableColumn("F3", SqlDbType.Int, true),
                new TableColumn("F4", SqlDbType.Int, true),
                new TableColumn("F5", SqlDbType.Int, true),
                new TableColumn("F6", SqlDbType.Int, true),
                new TableColumn("F7", SqlDbType.Int, true),
                new TableColumn("F8", SqlDbType.Int, true),
                new TableColumn("F9", SqlDbType.Int, true),
                new TableColumn("F10",SqlDbType.Int, true),});

                string sql_generate_Massendaten = @"
select top 0 F1,F2,F3,F4,F5,F6,F7,F8,F9,F10 into " + TempObjectNameName + @" from " + QuellObjekt + @" -- tmp-Tabelle erstellen
declare @grenze as int = " + SkalaGrenze + @"
declare @i as int = 0
while (@i < " + Anzahl_je_Faktor + @")
begin
	insert into test.tmp
	select @i % @grenze, @i % @grenze + 1, @i % @grenze + 2, (@i % @grenze) * -1, (@i % @grenze) * -1 -1, @i % @grenze, @i % @grenze -1, @i % @grenze +2, @i% @grenze+3, @i % @grenze+4
	set @i = @i + 1
end

declare @j as int = 0
while (@j < " + Anzahl_Faktoren + @")
begin
	insert into " + QuellObjekt + @"
	select F1,F2,F3,F4,F5,F6,F7,F8,F9,F10 from test.tmp
	set @j = @j + 1
end
"
    ;
                Debug.WriteLine("Generiere Massendaten ... ");

                new ExecuteSQLTask(TestDb.getNewSqlConnection()).ExecuteNonQuery(sql_generate_Massendaten);

                string ZielSchemaName = "test";
                string ZielTabelle = "destination";
                string ZielObjekt = $"[{ZielSchemaName}].[{ZielTabelle}]";
                new DropAndCreateTableTask(TestDb.getNewSqlConnection()).Execute(ZielSchemaName, ZielTabelle, new List<TableColumn>() {

               new TableColumn("Key", SqlDbType.Int, false, true, true),
                new TableColumn("F1", SqlDbType.Int, true), new TableColumn("F1_calc", SqlDbType.Int, true),
                new TableColumn("F2", SqlDbType.Int, true), new TableColumn("F2_calc", SqlDbType.Int, true),
                new TableColumn("F3", SqlDbType.Int, true), new TableColumn("F3_calc", SqlDbType.Int, true),
                new TableColumn("F4", SqlDbType.Int, true), new TableColumn("F4_calc", SqlDbType.Int, true),
                new TableColumn("F5", SqlDbType.Int, true), new TableColumn("F5_calc", SqlDbType.Int, true),
                new TableColumn("F6", SqlDbType.Int, true), new TableColumn("F6_calc", SqlDbType.Int, true),
                new TableColumn("F7", SqlDbType.Int, true), new TableColumn("F7_calc", SqlDbType.Int, true),
                new TableColumn("F8", SqlDbType.Int, true), new TableColumn("F8_calc", SqlDbType.Int, true),
                new TableColumn("F9", SqlDbType.Int, true), new TableColumn("F9_calc", SqlDbType.Int, true),
                new TableColumn("F10",SqlDbType.Int, true), new TableColumn("F10_calc", SqlDbType.Int, true),
});


                System.Data.SqlClient.SqlConnectionStringBuilder builder_CurrentDbConnection
                    = new System.Data.SqlClient.SqlConnectionStringBuilder(TestDb.getNewSqlConnection().ConnectionString);
                string Current_InitialCatalog = builder_CurrentDbConnection.InitialCatalog;
                string Current_DataSource = builder_CurrentDbConnection.DataSource;

                SqlSource<Datensatz> DBSource = new SqlSource<Datensatz>(TestDb.getNewSqlConnection()
                    , string.Format("select [Key],F1,F2,F3,F4,F5,F6,F7,F8,F9,F10 from {0}", QuellObjekt)
                    );
                DBSource.DataMappingMethod = ReaderAdapter.Read;


                SqlDestination<Datensatz> Ziel_Schreibe = new SqlDestination<Datensatz>();
                Ziel_Schreibe.ObjectName = ZielObjekt;
                Ziel_Schreibe.FieldCount = FieldCount;
                Ziel_Schreibe.ObjectMappingMethod = WriterAdapter.Fill;
                Ziel_Schreibe.SqlConnection = TestDb.SqlConnection;


                Graph g = new Graph();

                g.GetVertex(0, DBSource);
                g.GetVertex(1, new RowTransformation<Datensatz>(RowTransformationDB));
                g.GetVertex(2, Ziel_Schreibe);

                g.AddEdge(0, 1); // connect 0 to 1
                g.AddEdge(1, 2); // connect 1 to 2


                //TestHelper.VisualizeGraph(g);


                int MaxDegreeOfParallelism = 1;
                new ExecuteSQLTask(TestDb.getNewSqlConnection()).ExecuteNonQuery(string.Format("truncate table {0}", ZielObjekt));
                Debug.WriteLine("Start Laufzeittest MaxDegreeOfParallelism {0} ... ", MaxDegreeOfParallelism);
                Stopwatch s = Stopwatch.StartNew();
                DBSource.SqlConnection = TestDb.getNewSqlConnection();
                DataFlowTask<Datensatz>.Execute("Test dataflow task", 10000, MaxDegreeOfParallelism, g);
                Debug.WriteLine("Laufzeit in ms: {0}", s.ElapsedMilliseconds);

                MaxDegreeOfParallelism = 5;
                new ExecuteSQLTask(TestDb.getNewSqlConnection()).ExecuteNonQuery(string.Format("truncate table {0}", ZielObjekt));
                Debug.WriteLine("Start Laufzeittest MaxDegreeOfParallelism {0} ... ", MaxDegreeOfParallelism);
                s = Stopwatch.StartNew();
                DBSource.SqlConnection = TestDb.getNewSqlConnection();
                DataFlowTask<Datensatz>.Execute("Test dataflow task", 10000, MaxDegreeOfParallelism, g);
                Debug.WriteLine("Laufzeit in ms: {0}", s.ElapsedMilliseconds);


                MaxDegreeOfParallelism = 10;
                new ExecuteSQLTask(TestDb.getNewSqlConnection()).ExecuteNonQuery(string.Format("truncate table {0}", ZielObjekt));
                Debug.WriteLine("Start Laufzeittest MaxDegreeOfParallelism {0} ... ", MaxDegreeOfParallelism);
                s = Stopwatch.StartNew();
                DBSource.SqlConnection = TestDb.getNewSqlConnection();
                DataFlowTask<Datensatz>.Execute("Test dataflow task", 10000, MaxDegreeOfParallelism, g);
                Debug.WriteLine("Laufzeit in ms: {0}", s.ElapsedMilliseconds);



                Assert.AreEqual(Anzahl_je_Faktor * Anzahl_Faktoren, new ExecuteSQLTask(TestDb.SqlConnection).ExecuteScalar(string.Format("select count(*) from {0}", QuellObjekt)));

            }
        }



    }
}
