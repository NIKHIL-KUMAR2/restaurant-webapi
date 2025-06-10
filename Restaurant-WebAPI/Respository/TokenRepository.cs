using System;
using System.Data;
using Restaurant_WebAPI.Util;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Repository
{
    public class TokenRepository : ITokenRepository
    {
        public void SaveRefreshToken(string userId, string token, DateTime expiresUtc)
        {
            string query = @"INSERT INTO RefreshToken (Token, Subject, ExpiresUtc)
                             VALUES (@Token, @Subject, @ExpiresUtc)";

            var parameters = new[]
            {
                DBUtil.CreateParameter("@Token", DbType.String, token),
                DBUtil.CreateParameter("@Subject", DbType.String, userId),
                DBUtil.CreateParameter("@ExpiresUtc", DbType.DateTime, expiresUtc)
            };

            DBUtil.ExecuteNonQuery(query, parameters);
        }

        public RefreshToken GetRefreshToken(string token)
        {
            string query = @"SELECT * FROM RefreshToken WHERE Token = @Token";

            var parameter = DBUtil.CreateParameter("@Token", DbType.String, token);

            using (var reader = DBUtil.ExecuteReader(query, parameter))
            {
                if (reader.Read())
                {
                    return new RefreshToken
                    {
                        Token = reader["Token"].ToString(),
                        Subject = reader["Subject"].ToString(),
                        ExpiresUtc = Convert.ToDateTime(reader["ExpiresUtc"])
                    };
                }
            }
            return null;
        }

        public int RemoveRefreshToken(string token)
        {
            string query = @"DELETE FROM RefreshToken WHERE Token = @Token";

            var parameter = DBUtil.CreateParameter("@Token", DbType.String, token);

            return DBUtil.ExecuteNonQuery(query, parameter);
        }

        public int RemoveRefreshTokenByUserId(string userId)
        {
            string query = @"DELETE FROM RefreshToken WHERE Subject = @UserId";

            var parameter = DBUtil.CreateParameter("@UserId", DbType.String, userId);

            return DBUtil.ExecuteNonQuery(query, parameter);
        }
    }
}
