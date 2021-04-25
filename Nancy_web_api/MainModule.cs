using Nancy;
using Nancy.Extensions;
using Nancy_web_api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class MainModule : NancyModule
{
    public SQLiteConnection theConnection = new SQLiteConnection(@"Data Source=.\Api_DB.db;Version=3;");
    DateTime actionDate = DateTime.Now;
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public MainModule()
    {
        Get("/getClientList", args =>
        {
            theConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand(theConnection);
            cmd.CommandText = @"SELECT Id, Age, Name, Comment FROM Client";
            cmd.CommandType = System.Data.CommandType.Text;
            SQLiteDataReader r = cmd.ExecuteReader();
            var clients = new List<Client>();
            while (r.Read())
            {
                object x = r["Age"];
                var a = (x.GetType().Name);
                if (r["Id"] != null)
                {
                    clients.Add(new Client
                    {
                        Age = (long)r["Age"],
                        Name = (string)r["Name"],
                        Comment = r["Comment"].GetType() != typeof(DBNull) ?
                        (string)r["Comment"] : null,
                        Id = (long)r["Id"]


                    });
                }
            }
            log.Info("Client List retrieved");
            theConnection.Close();
            return clients;
        });
        Get("/getClient/{id}", args =>
        {
            theConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand(theConnection);
            cmd.CommandText = $"SELECT Id, Age, Name, Comment FROM Client WHERE Id = {args.id}";
            cmd.CommandType = System.Data.CommandType.Text;
            SQLiteDataReader r = cmd.ExecuteReader();
            var client = new Client();
            while (r.Read())
            {
                object x = r["Age"];
                var a = (x.GetType().Name);
                if (r["Id"] != null)
                {


                    client.Age = (long)r["Age"];
                    client.Name = (string)r["Name"];
                    client.Comment = r["Comment"].GetType() != typeof(DBNull) ?
                    (string)r["Comment"] : null;
                    client.Id = (long)r["Id"];
                }
            }
            log.Info($"Client, id number - {args.id}, retrieved");
            theConnection.Close();
            return client;
        });

        Post("/postClient", _ =>
        {
            var jsonString = Request.Body.AsString();
            var client = JsonConvert.DeserializeObject<Client>(jsonString);
            theConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand(theConnection);
            SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO Client (Name, Age, Comment) VALUES(@Name, @Age, @Comment)", theConnection);
            insertSQL.Parameters.AddWithValue("@Name", client.Name);
            insertSQL.Parameters.AddWithValue("@Age", client.Age);
            insertSQL.Parameters.AddWithValue("@Comment", client.Comment);
            insertSQL.ExecuteNonQuery();
            log.Info($" Client {client.Name}, age: {client.Age}, added");
            LogClientHistory(ClientHistoryStatus.Registered, theConnection.LastInsertRowId);
            theConnection.Close();
       

            return "Klientas pridetas!";
        });
        Put("/putClientUpdate", _ =>
        {
            var jsonString = Request.Body.AsString();
            var client = JsonConvert.DeserializeObject<Client>(jsonString);
            theConnection.Open();
            SQLiteCommand cmd = new SQLiteCommand(theConnection);
            SQLiteCommand updateClient = new SQLiteCommand("UPDATE Client SET Name = @newName , Age = @newAge , Comment = @newComment WHERE Id = @Id;", theConnection);
            updateClient.Parameters.AddWithValue("@newName", client.Name);
            updateClient.Parameters.AddWithValue("@newAge", client.Age);
            updateClient.Parameters.AddWithValue("@newComment", client.Comment);
            updateClient.Parameters.AddWithValue("@Id", client.Id);
            updateClient.ExecuteNonQuery();
            LogClientHistory(ClientHistoryStatus.Updated, client.Id);
            theConnection.Close();
            return "Irasas atnaujintas";
        });
        Delete("/deleteClient", _ =>
        {
            var jsonString = Request.Body.AsString();
            var client = JsonConvert.DeserializeObject<Client>(jsonString);
            SQLiteCommand cmd = new SQLiteCommand(theConnection);
            theConnection.Open();
            SQLiteCommand delClient = new SQLiteCommand("DELETE FROM Client WHERE Id = @Id ", theConnection);
            delClient.Parameters.AddWithValue("@Id", client.Id);
            log.Info($"User, id number - {client.Id} was deleted!");
            try
            {
                delClient.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            SQLiteCommand delHistory = new SQLiteCommand($"DELETE FROM Client_history WHERE ClientId = @Id", theConnection);
            delHistory.Parameters.AddWithValue("@Id", client.Id);
            try
            {
                delHistory.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            theConnection.Close();
            return "Klientas istrintas!";
        });
    }

    private void LogClientHistory(ClientHistoryStatus action, long clientId)
    {
        SQLiteCommand cmd = new SQLiteCommand(theConnection);
        var updateClient = new SQLiteCommand("INSERT INTO Client_history(Action, ActionDate, ClientId) VALUES(@Action, @ActionDate, @ClientId)", theConnection);
        updateClient.Parameters.AddWithValue("@Action", action);
        updateClient.Parameters.AddWithValue("@ActionDate", actionDate);
        updateClient.Parameters.AddWithValue("@ClientId", clientId);
        updateClient.ExecuteNonQuery();
        log.Info($"Client history entry created: {clientId} , {action}, {actionDate}");
    }
}
