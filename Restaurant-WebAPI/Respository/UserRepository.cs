using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Restaurant_WebAPI.Enums;
using Restaurant_WebAPI.Exceptions;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;
using Restaurant_WebAPI.Util;

namespace Restaurant_WebAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        public UserModel GetUserByEmail(string email)
        {
            string query = @"SELECT * FROM AppUser WHERE email = @Email;";
            var parameters = new NpgsqlParameter[]
            {
                DBUtil.CreateParameter("@Email", DbType.String, email)
            };

            using (var reader = DBUtil.ExecuteReader(query, parameters))
            {
                if (reader.Read())
                {
                    return new UserModel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        FirstName = reader["firstName"].ToString(),
                        Email = reader["email"].ToString(),
                        PasswordHash = reader["password"].ToString(),
                        Role = (UserRole)Convert.ToInt32(reader["role"]),
                        IsActive = Convert.ToBoolean(reader["isActive"])
                    };
                }
            }
            return null;
        }

        public int AddNewUser(UserModel model)
        {
            try
            {
                string query = @"
                    INSERT INTO AppUser 
                    (firstName, lastName, email, password, role)
                    VALUES (@FirstName, @LastName, @Email, @Password, @Role);";

                var parameters = new NpgsqlParameter[]
                {
                    DBUtil.CreateParameter("@FirstName", DbType.String, model.FirstName),
                    DBUtil.CreateParameter("@LastName", DbType.String, model.LastName),
                    DBUtil.CreateParameter("@Email", DbType.String, model.Email),
                    DBUtil.CreateParameter("@Password", DbType.String, model.PasswordHash),
                    DBUtil.CreateParameter("@Role", DbType.Int32, 1)
                };

                return DBUtil.ExecuteNonQuery(query, parameters);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // Unique violation
            {
                throw new DatabaseException("User Already Exist.");
            }
        }

        public int UpdateUser(UpdateRequest request, int userId)
        {
            try
            {
                var updateFields = new List<string>();
                var parameters = new List<NpgsqlParameter>();

                if (!string.IsNullOrEmpty(request.Email))
                {
                    updateFields.Add(@"email = @Email");
                    parameters.Add(DBUtil.CreateParameter("@Email", DbType.String, request.Email));
                }

                if (!string.IsNullOrEmpty(request.FirstName))
                {
                    updateFields.Add(@"firstName = @FirstName");
                    parameters.Add(DBUtil.CreateParameter("@FirstName", DbType.String, request.FirstName));
                }

                if (!string.IsNullOrEmpty(request.LastName))
                {
                    updateFields.Add(@"lastName = @LastName");
                    parameters.Add(DBUtil.CreateParameter("@LastName", DbType.String, request.LastName));
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    updateFields.Add(@"password = @Password");
                    parameters.Add(DBUtil.CreateParameter("@Password", DbType.String, request.Password));
                }

                if (updateFields.Count == 0)
                    throw new AccountRequestException("No valid fields provided for update.");

                string query = $@"UPDATE AppUser SET {string.Join(", ", updateFields)} WHERE id = @UserId;";
                parameters.Add(DBUtil.CreateParameter("@UserId", DbType.Int32, userId));

                return DBUtil.ExecuteNonQuery(query, parameters.ToArray());
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // Unique violation
            {
                throw new DatabaseException("User Already Exist.");
            }
        }

        public int DeactivateUser(int userId)
        {
            string query = @"UPDATE AppUser SET isActive = FALSE WHERE id = @UserId AND isActive = TRUE;";
            return DBUtil.ExecuteNonQuery(query, DBUtil.CreateParameter("@UserId", DbType.Int32, userId));
        }
    }
}
