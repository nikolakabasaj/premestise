﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Persistence.Interfaces.Contracts;
using Persistence.Interfaces.Entites;

namespace Persistence.Repositories
{
    public class PendingRequestRepository : IPendingRequestRepository
    {
        private readonly string _connectionString;

        public PendingRequestRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public PendingRequest Create(PendingRequest request)
        {
            request.SubmittedAt = DateTime.Now;

            SqlCommand command = new SqlCommand();
            command.CommandText = $@"INSERT INTO pending_request (parent_email, parent_phone_number, child_name, child_birth_date, from_kindergarden_id, submitted_at) 
                                    VALUES (@email, @phoneNumber, @childName, @childBirthDate, @fromKindergardenId, @submittedAt)
                                    SELECT SCOPE_IDENTITY()";

            command.Parameters.Add("@title", SqlDbType.NVarChar).Value = request.ParentEmail;
            command.Parameters.Add("@childName", SqlDbType.NVarChar).Value = request.ParentPhoneNumber;
            command.Parameters.Add("@childName", SqlDbType.NVarChar).Value = request.ChildName;
            command.Parameters.Add("@childBirthDate", SqlDbType.DateTime).Value = request.ChildBirthDate;
            command.Parameters.Add("@fromKindergardenId", SqlDbType.Int).Value = request.FromKindergardenId;
            command.Parameters.Add("@submittedAt", SqlDbType.NVarChar).Value = request.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss");

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    int id = (int)command.ExecuteScalar();
                    request.Id = id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return request;
        }

        public void Delete(int id)
        {
            SqlCommand command = new SqlCommand($"DELETE FROM pending_request WHERE ID = @id");
            command.Parameters.Add("@id", SqlDbType.Int).Value = id;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public IEnumerable<PendingRequest> GetAllMatchesFor(PendingRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
