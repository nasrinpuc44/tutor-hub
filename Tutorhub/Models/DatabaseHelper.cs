// 📁 Models/DatabaseHelper.cs
// লোকেশন: Tutorbub/Models/DatabaseHelper.cs

using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tutorbub.Models;

namespace Tutorbub.Models
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Database")
                ?? throw new InvalidOperationException("Connection string 'Database' not found.");
        }

        // ============================================================
        // ===== USER RELATED METHODS =====
        // ============================================================

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""TotalPoints"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                ORDER BY ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(MapUser(reader));
                }
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all users: " + ex.Message);
            }
        }

        public User? AuthenticateUser(string username, string password)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""TotalPoints"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                WHERE LOWER(""UserName"") = LOWER(@username) 
                  AND ""Password"" = @password 
                  AND ""IsActive"" = TRUE";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user: " + ex.Message);
            }
        }

        public bool UpdateUserProfile(User user)
        {
            string query = @"
                UPDATE ""Users"" SET 
                    ""FullName"" = @fullName,
                    ""Email"" = @email,
                    ""MobileNumber"" = @mobileNumber,
                    ""Gender"" = @gender,
                    ""AgeRange"" = @ageRange,
                    ""PrimaryDeviceType"" = @primaryDeviceType,
                    ""YearsOfExperience"" = @yearsOfExperience,
                    ""AreaType"" = @areaType,
                    ""Country"" = @country,
                    ""StreetAddress"" = @streetAddress,
                    ""PermanentAddress"" = @permanentAddress,
                    ""EducationLevel"" = @educationLevel,
                    ""CurrentStudyStatus"" = @currentStudyStatus,
                    ""ExamDegreeTitle"" = @examDegreeTitle,
                    ""InstitutionName"" = @institutionName,
                    ""PassingYear"" = @passingYear,
                    ""IsCSEStudent"" = @isCSEStudent,
                    ""CvLink"" = @cvLink,
                    ""GithubProfile"" = @githubProfile,
                    ""PortfolioLink"" = @portfolioLink,
                    ""LinkedInProfile"" = @linkedInProfile,
                    ""ProfileImageLink"" = @profileImageLink
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@fullName", user.FullName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@mobileNumber", user.MobileNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gender", user.Gender ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ageRange", user.AgeRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@primaryDeviceType", user.PrimaryDeviceType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@yearsOfExperience", user.YearsOfExperience ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@areaType", user.AreaType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@country", user.Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@streetAddress", user.StreetAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@permanentAddress", user.PermanentAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@educationLevel", user.EducationLevel ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@currentStudyStatus", user.CurrentStudyStatus ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@examDegreeTitle", user.ExamDegreeTitle ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@institutionName", user.InstitutionName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@passingYear", user.PassingYear ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCSEStudent", user.IsCSEStudent ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cvLink", user.CvLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@githubProfile", user.GithubProfile ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@portfolioLink", user.PortfolioLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@linkedInProfile", user.LinkedInProfile ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@profileImageLink", user.ProfileImageLink ?? (object)DBNull.Value);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user profile: " + ex.Message);
            }
        }

        public User? GetUserById(int userId)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""TotalPoints"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user by ID: " + ex.Message);
            }
        }

        public bool RegisterUser(User user)
        {
            var trimmedEmail = (user.Email ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(trimmedEmail) ||
                !trimmedEmail.EndsWith("@gmail.com"))
            {
                throw new Exception("Email must end with @gmail.com (all lowercase).");
            }

            var gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@gmail\.com$");
            if (!gmailRegex.IsMatch(trimmedEmail))
            {
                throw new Exception("Invalid Gmail address format.");
            }

            string query = @"
                INSERT INTO ""Users"" (""UserName"", ""Password"", ""FullName"", ""Email"", ""Role"", ""TotalPoints"")
                VALUES (@username, @password, @fullname, @email, 'User', 0)";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", user.UserName);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@fullname", user.FullName);
                command.Parameters.AddWithValue("@email", trimmedEmail);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user: " + ex.Message);
            }
        }

        public bool DeleteUser(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using (var deleteRequestsCommand = new NpgsqlCommand(
                    "DELETE FROM \"TeacherRequests\" WHERE \"UserId\" = @id", connection, transaction))
                {
                    deleteRequestsCommand.Parameters.AddWithValue("@id", userId);
                    deleteRequestsCommand.ExecuteNonQuery();
                }

                using var deleteUserCommand = new NpgsqlCommand(
                    "DELETE FROM \"Users\" WHERE \"Id\" = @id", connection, transaction);
                deleteUserCommand.Parameters.AddWithValue("@id", userId);
                int rowsAffected = deleteUserCommand.ExecuteNonQuery();

                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error deleting user: " + ex.Message);
            }
        }

        public bool UpdatePassword(int userId, string newPassword)
        {
            string query = "UPDATE \"Users\" SET \"Password\" = @password WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@password", newPassword);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating password: " + ex.Message);
            }
        }

        public bool ToggleUserStatus(int userId, bool isActive)
        {
            string query = "UPDATE \"Users\" SET \"IsActive\" = @isActive WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error toggling user status: " + ex.Message);
            }
        }

        public bool UsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM \"Users\" WHERE LOWER(\"UserName\") = LOWER(@username)";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking username: " + ex.Message);
            }
        }

        public bool EmailExists(string email)
        {
            var trimmed = (email ?? string.Empty).Trim();

            string query = "SELECT COUNT(*) FROM \"Users\" WHERE LOWER(\"Email\") = LOWER(@email)";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", trimmed);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking email: " + ex.Message);
            }
        }

        public void UpdateLastLogin(string username)
        {
            string query = @"UPDATE ""Users"" SET ""LastLoginAt"" = @lastLogin WHERE ""UserName"" = @username";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@lastLogin", DateTime.UtcNow);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating last login: " + ex.Message);
            }
        }

        // ============================================================
        // ===== ✅ POINTS SYSTEM =====
        // ============================================================

        public bool AddPoints(int userId, int points)
        {
            string query = @"
                UPDATE ""Users"" 
                SET ""TotalPoints"" = GREATEST(0, COALESCE(""TotalPoints"", 0) + @points)
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@points", points);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding points: " + ex.Message);
                return false;
            }
        }

        public int GetUserPoints(int userId)
        {
            string query = @"SELECT COALESCE(""TotalPoints"", 0) FROM ""Users"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user points: " + ex.Message);
                return 0;
            }
        }

        public bool SaveQuizAttemptAndAwardPoints(QuizAttempt attempt, out string? errorMessage)
        {
            errorMessage = null;

            if (!SaveQuizAttempt(attempt, out string? saveError))
            {
                errorMessage = saveError;
                return false;
            }

            if (attempt.Score > 0)
            {
                AddPoints(attempt.UserId, attempt.Score);
            }

            return true;
        }

        // ============================================================
        // ===== ✅ POINTS TRANSACTION LOG SYSTEM =====
        // ============================================================

        public bool LogPointsTransaction(int userId, int points, string type,
            string? reference = null, string? description = null)
        {
            string query = @"
                INSERT INTO ""PointsTransactions"" 
                (""UserId"", ""Points"", ""Type"", ""Reference"", ""Description"", ""CreatedAt"")
                VALUES (@userId, @points, @type, @reference, @description, @createdAt)";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@points", points);
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@reference", reference ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error LogPointsTransaction: " + ex.Message);
                return false;
            }
        }

        public List<PointsTransaction> GetPointsHistory(int userId, int limit = 50)
        {
            var list = new List<PointsTransaction>();
            string query = @"
                SELECT ""Id"", ""UserId"", ""Points"", ""Type"", ""Reference"", ""Description"", ""CreatedAt""
                FROM ""PointsTransactions""
                WHERE ""UserId"" = @userId
                ORDER BY ""CreatedAt"" DESC
                LIMIT @limit";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@limit", limit);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new PointsTransaction
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Points = Convert.ToInt32(reader["Points"]),
                        Type = reader["Type"]?.ToString() ?? "info",
                        Reference = reader["Reference"]?.ToString(),
                        Description = reader["Description"]?.ToString(),
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetPointsHistory: " + ex.Message);
                return list;
            }
        }

        public bool SpendPoints(int userId, int points, string type, string reference,
            string description, out string? errorMessage)
        {
            errorMessage = null;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // 1) Current points check (with row lock)
                    int currentPoints;
                    using (var checkCmd = new NpgsqlCommand(
                        @"SELECT COALESCE(""TotalPoints"", 0) FROM ""Users"" WHERE ""Id"" = @id FOR UPDATE",
                        connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@id", userId);
                        var result = checkCmd.ExecuteScalar();
                        currentPoints = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }

                    if (currentPoints < points)
                    {
                        errorMessage = $"Not enough points. You have {currentPoints}, need {points}.";
                        transaction.Rollback();
                        return false;
                    }

                    // 2) Deduct points
                    using (var deductCmd = new NpgsqlCommand(
                        @"UPDATE ""Users"" SET ""TotalPoints"" = ""TotalPoints"" - @points WHERE ""Id"" = @id",
                        connection, transaction))
                    {
                        deductCmd.Parameters.AddWithValue("@points", points);
                        deductCmd.Parameters.AddWithValue("@id", userId);
                        deductCmd.ExecuteNonQuery();
                    }

                    // 3) Log transaction
                    using (var logCmd = new NpgsqlCommand(
                        @"INSERT INTO ""PointsTransactions"" 
                          (""UserId"", ""Points"", ""Type"", ""Reference"", ""Description"", ""CreatedAt"")
                          VALUES (@userId, @points, @type, @reference, @description, @createdAt)",
                        connection, transaction))
                    {
                        logCmd.Parameters.AddWithValue("@userId", userId);
                        logCmd.Parameters.AddWithValue("@points", -points);
                        logCmd.Parameters.AddWithValue("@type", type);
                        logCmd.Parameters.AddWithValue("@reference", reference ?? (object)DBNull.Value);
                        logCmd.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
                        logCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                        logCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine($"✅ Spent {points} pts from user {userId} — {description}");
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Console.WriteLine("Error SpendPoints: " + ex.Message);
                return false;
            }
        }

        public bool AddPointsWithLog(int userId, int points, string type, string reference, string description)
        {
            if (points <= 0) return false;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // 1) Add points
                    using (var addCmd = new NpgsqlCommand(
                        @"UPDATE ""Users"" 
                          SET ""TotalPoints"" = GREATEST(0, COALESCE(""TotalPoints"", 0) + @points)
                          WHERE ""Id"" = @id",
                        connection, transaction))
                    {
                        addCmd.Parameters.AddWithValue("@points", points);
                        addCmd.Parameters.AddWithValue("@id", userId);
                        addCmd.ExecuteNonQuery();
                    }

                    // 2) Log transaction
                    using (var logCmd = new NpgsqlCommand(
                        @"INSERT INTO ""PointsTransactions"" 
                          (""UserId"", ""Points"", ""Type"", ""Reference"", ""Description"", ""CreatedAt"")
                          VALUES (@userId, @points, @type, @reference, @description, @createdAt)",
                        connection, transaction))
                    {
                        logCmd.Parameters.AddWithValue("@userId", userId);
                        logCmd.Parameters.AddWithValue("@points", points);
                        logCmd.Parameters.AddWithValue("@type", type);
                        logCmd.Parameters.AddWithValue("@reference", reference ?? (object)DBNull.Value);
                        logCmd.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
                        logCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                        logCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error AddPointsWithLog: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== TEACHER REQUEST RELATED METHODS =====
        // ============================================================

        public bool CreateTeacherRequest(TeacherRequest request, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""TeacherRequests"" 
                (""UserId"", ""FullName"", ""Email"", ""MobileNumber"", ""Education"", ""Institution"", 
                 ""SubjectExpertise"", ""Experience"", ""TeachingStyle"", ""AvailableDays"", ""PreferredTime"", 
                 ""HourlyRate"", ""CvLink"", ""WhyTeach"", ""Status"", ""RequestDate"")
                VALUES 
                (@userId, @fullName, @email, @mobileNumber, @education, @institution, 
                 @subjectExpertise, @experience, @teachingStyle, @availableDays, @preferredTime, 
                 @hourlyRate, @cvLink, @whyTeach, 'Pending', @requestDate)";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", request.UserId);
                command.Parameters.AddWithValue("@fullName", request.FullName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", request.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@mobileNumber", request.MobileNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@education", request.Education ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@institution", request.Institution ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@subjectExpertise", request.SubjectExpertise ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@experience", request.Experience ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@teachingStyle", request.TeachingStyle ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@availableDays", request.AvailableDays ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@preferredTime", request.PreferredTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@hourlyRate", request.HourlyRate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cvLink", request.CvLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@whyTeach", request.WhyTeach ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@requestDate", request.RequestDate);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public string GetUserTeacherRequestStatus(int userId)
        {
            string query = @"SELECT ""Status"" FROM ""TeacherRequests"" 
                             WHERE ""UserId"" = @userId 
                             ORDER BY ""RequestDate"" DESC LIMIT 1";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();

                var result = command.ExecuteScalar();
                return result?.ToString() ?? "None";
            }
            catch
            {
                return "None";
            }
        }

        public List<TeacherRequest> GetAllTeacherRequests()
        {
            var requests = new List<TeacherRequest>();
            string query = @"SELECT * FROM ""TeacherRequests"" ORDER BY ""RequestDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    requests.Add(MapTeacherRequest(reader));
                }
                return requests;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting teacher requests: " + ex.Message);
            }
        }

        public bool UpdateTeacherRequest(int requestId, string status, string? adminNote)
        {
            string query = @"UPDATE ""TeacherRequests"" 
                             SET ""Status"" = @status, ""ResponseDate"" = @responseDate, ""AdminNote"" = @adminNote
                             WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@responseDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@adminNote", adminNote ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@id", requestId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating teacher request: " + ex.Message);
            }
        }

        public TeacherRequest? GetTeacherRequestById(int requestId)
        {
            string query = @"SELECT * FROM ""TeacherRequests"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", requestId);
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapTeacherRequest(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting teacher request: " + ex.Message);
            }
        }

        public bool MakeUserTeacher(int userId)
        {
            string query = @"UPDATE ""Users"" SET ""Role"" = 'Teacher' WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error making user teacher: " + ex.Message);
            }
        }

        public bool DeleteTeacherRequestById(int requestId)
        {
            string query = "DELETE FROM \"TeacherRequests\" WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", requestId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting teacher request: " + ex.Message);
            }
        }

        // ============================================================
        // ===== FIND TUTOR — APPROVED TEACHERS =====
        // ============================================================
        public List<TutorProfile> GetApprovedTutors()
        {
            var tutors = new List<TutorProfile>();

            string query = @"
                SELECT 
                    ""Id"", ""UserId"", ""FullName"", ""Email"", ""MobileNumber"",
                    ""Education"", ""Institution"", ""SubjectExpertise"", ""Experience"",
                    ""TeachingStyle"", ""AvailableDays"", ""PreferredTime"", ""HourlyRate"",
                    ""CvLink"", ""WhyTeach"", ""Status"", ""RequestDate""
                FROM ""TeacherRequests""
                WHERE ""Status"" = 'Approved'
                ORDER BY ""RequestDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();

                using var reader = command.ExecuteReader();
                int index = 1;

                while (reader.Read())
                {
                    var fullName = reader["FullName"]?.ToString() ?? "Tutor";
                    var requestId = reader.GetInt32(reader.GetOrdinal("Id"));
                    var tutorId = "TCH-" + (1000 + requestId);

                    var words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string initials = "T";
                    if (words.Length >= 2)
                        initials = words[0][0].ToString().ToUpper() + words[1][0].ToString().ToUpper();
                    else if (words.Length == 1 && words[0].Length > 0)
                        initials = words[0][0].ToString().ToUpper();

                    var subjectExpertise = reader["SubjectExpertise"]?.ToString() ?? "";
                    var subjects = subjectExpertise
                        .Split(new[] { ',', '/', '&' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
                    if (subjects.Count == 0) subjects.Add("General");

                    var classes = new List<string> { "All Classes" };

                    int expYears = 0;
                    var expStr = reader["Experience"]?.ToString() ?? "";
                    var expDigits = new string(expStr.Where(char.IsDigit).ToArray());
                    if (!string.IsNullOrEmpty(expDigits))
                    {
                        var numStr = expDigits.Length > 2 ? expDigits.Substring(0, 2) : expDigits;
                        int.TryParse(numStr, out expYears);
                    }

                    double rating = Math.Round(4.5 + (index % 5) * 0.1, 1);
                    int reviews = 20 + index * 3;

                    decimal monthlyFee = 5000 + (index % 6) * 1000;
                    var hourlyRateStr = reader["HourlyRate"]?.ToString() ?? "";
                    if (decimal.TryParse(hourlyRateStr, out decimal hourlyRate) && hourlyRate > 0)
                    {
                        monthlyFee = Math.Round(hourlyRate * 30, 0);
                    }

                    tutors.Add(new TutorProfile
                    {
                        Id = requestId,
                        TutorId = tutorId,
                        FullName = fullName,
                        Initials = initials,
                        Gender = "Any",
                        PhotoUrl = "",
                        Subjects = subjects,
                        Classes = classes,
                        Qualification = reader["Education"]?.ToString() ?? "Not specified",
                        Institution = reader["Institution"]?.ToString() ?? "Not specified",
                        ExperienceYears = expYears,
                        Location = "Bangladesh",
                        TeachingMode = "Both",
                        MonthlyFee = monthlyFee,
                        Rating = rating,
                        TotalReviews = reviews,
                        AvailableTime = !string.IsNullOrEmpty(reader["PreferredTime"]?.ToString())
                            ? reader["PreferredTime"].ToString()!
                            : (reader["AvailableDays"]?.ToString() ?? "Contact for schedule"),
                        About = !string.IsNullOrEmpty(reader["WhyTeach"]?.ToString())
                            ? reader["WhyTeach"].ToString()!
                            : (!string.IsNullOrEmpty(reader["TeachingStyle"]?.ToString())
                                ? reader["TeachingStyle"].ToString()!
                                : "Experienced tutor ready to help students excel."),
                        IsVerified = true,
                        IsTopRated = index <= 3
                    });

                    index++;
                }

                Console.WriteLine($"GetApprovedTutors: {tutors.Count} approved tutors found");
                return tutors;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting approved tutors: " + ex.Message);
                return new List<TutorProfile>();
            }
        }

        // ============================================================
        // ===== COURSE RELATED METHODS =====
        // ============================================================

        public bool CreateCourse(Course course, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""Courses"" 
                (""Title"", ""Description"", ""Image"", ""Category"", ""Price"", 
                 ""Instructor"", ""Duration"", ""Lessons"", ""Level"", ""IsNew"", ""IsPopular"", 
                 ""IsEnrollmentOpen"", ""Features"", ""CreatedAt"")
                VALUES 
                (@title, @description, @image, @category, @price,
                 @instructor, @duration, @lessons, @level, @isNew, @isPopular, 
                 @isEnrollmentOpen, @features, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@title", course.Title ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@description", course.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@image", course.Image ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@category", course.Category ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@price", course.Price);
                command.Parameters.AddWithValue("@instructor", course.Instructor ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@duration", course.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@lessons", course.Lessons);
                command.Parameters.AddWithValue("@level", course.Level ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isNew", course.IsNew);
                command.Parameters.AddWithValue("@isPopular", course.IsPopular);
                command.Parameters.AddWithValue("@isEnrollmentOpen", course.IsEnrollmentOpen);
                command.Parameters.AddWithValue("@features", course.Features ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", course.CreatedAt);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    course.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<Course> GetAllCourses()
        {
            var courses = new List<Course>();
            string query = @"SELECT * FROM ""Courses"" ORDER BY ""CreatedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    courses.Add(MapCourse(reader));
                }
                return courses;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting courses: " + ex.Message);
                return new List<Course>();
            }
        }

        public Course? GetCourseById(int id)
        {
            string query = @"SELECT * FROM ""Courses"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapCourse(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting course by id: " + ex.Message);
                return null;
            }
        }

        public bool DeleteCourse(int courseId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using (var deleteQuizQuestions = new NpgsqlCommand(
                    @"DELETE FROM ""QuizQuestions"" WHERE ""QuizId"" IN (SELECT ""Id"" FROM ""ModuleQuizzes"" WHERE ""CourseId"" = @id)", connection))
                {
                    deleteQuizQuestions.Parameters.AddWithValue("@id", courseId);
                    deleteQuizQuestions.ExecuteNonQuery();
                }
                using (var deleteQuizAttempts = new NpgsqlCommand(
                    @"DELETE FROM ""QuizAttempts"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteQuizAttempts.Parameters.AddWithValue("@id", courseId);
                    deleteQuizAttempts.ExecuteNonQuery();
                }
                using (var deleteQuizzes = new NpgsqlCommand(
                    @"DELETE FROM ""ModuleQuizzes"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteQuizzes.Parameters.AddWithValue("@id", courseId);
                    deleteQuizzes.ExecuteNonQuery();
                }
                using (var deleteAssignments = new NpgsqlCommand(
                    @"DELETE FROM ""MilestoneAssignments"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteAssignments.Parameters.AddWithValue("@id", courseId);
                    deleteAssignments.ExecuteNonQuery();
                }
                using (var deleteLessons = new NpgsqlCommand(
                    @"DELETE FROM ""CourseLessons"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteLessons.Parameters.AddWithValue("@id", courseId);
                    deleteLessons.ExecuteNonQuery();
                }
                using (var deleteLessonCompletions = new NpgsqlCommand(
                    @"DELETE FROM ""LessonCompletions"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteLessonCompletions.Parameters.AddWithValue("@id", courseId);
                    deleteLessonCompletions.ExecuteNonQuery();
                }
                using (var deleteOrders = new NpgsqlCommand(
                    @"DELETE FROM ""CourseOrders"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteOrders.Parameters.AddWithValue("@id", courseId);
                    deleteOrders.ExecuteNonQuery();
                }
                using (var deleteConfig = new NpgsqlCommand(
                    @"DELETE FROM ""CourseMilestoneConfig"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteConfig.Parameters.AddWithValue("@id", courseId);
                    deleteConfig.ExecuteNonQuery();
                }

                using var deleteCourse = new NpgsqlCommand(
                    @"DELETE FROM ""Courses"" WHERE ""Id"" = @id", connection);
                deleteCourse.Parameters.AddWithValue("@id", courseId);
                return deleteCourse.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting course: " + ex.Message);
                return false;
            }
        }

        public bool ToggleEnrollment(int courseId, bool isOpen)
        {
            string query = @"UPDATE ""Courses"" 
                             SET ""IsEnrollmentOpen"" = @isOpen 
                             WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@isOpen", isOpen);
                command.Parameters.AddWithValue("@id", courseId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error toggling enrollment: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== COURSE LESSON / VIDEO METHODS =====
        // ============================================================

        public bool CreateCourseLesson(CourseLesson lesson, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""CourseLessons"" 
                (""CourseId"", ""ModuleNumber"", ""LessonNumber"", ""Title"", 
                 ""VideoUrl"", ""Duration"", ""Description"", ""CreatedAt"")
                VALUES 
                (@courseId, @moduleNumber, @lessonNumber, @title,
                 @videoUrl, @duration, @description, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@courseId", lesson.CourseId);
                command.Parameters.AddWithValue("@moduleNumber", lesson.ModuleNumber);
                command.Parameters.AddWithValue("@lessonNumber", lesson.LessonNumber);
                command.Parameters.AddWithValue("@title", lesson.Title ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@videoUrl", lesson.VideoUrl ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@duration", lesson.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@description", lesson.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", lesson.CreatedAt);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    lesson.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<CourseLesson> GetLessonsByCourseId(int courseId)
        {
            var lessons = new List<CourseLesson>();
            string query = @"SELECT * FROM ""CourseLessons"" 
                             WHERE ""CourseId"" = @courseId 
                             ORDER BY ""ModuleNumber"", ""LessonNumber""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@courseId", courseId);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    lessons.Add(new CourseLesson
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        ModuleNumber = Convert.ToInt32(reader["ModuleNumber"]),
                        LessonNumber = Convert.ToInt32(reader["LessonNumber"]),
                        Title = reader["Title"]?.ToString() ?? "",
                        VideoUrl = reader["VideoUrl"]?.ToString() ?? "",
                        Duration = reader["Duration"]?.ToString() ?? "",
                        Description = reader["Description"]?.ToString(),
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow
                    });
                }
                return lessons;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting lessons: " + ex.Message);
                return new List<CourseLesson>();
            }
        }

        public bool DeleteCourseLesson(int lessonId)
        {
            string query = "DELETE FROM \"CourseLessons\" WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", lessonId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting lesson: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== PAYMENT / COURSE ORDER RELATED METHODS =====
        // ============================================================

        public bool CreateCourseOrder(CourseOrder order, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""CourseOrders"" 
                (""UserId"", ""CourseId"", ""CourseName"", ""CourseImage"", ""Price"", 
                 ""PaymentMethod"", ""TransactionId"", ""SenderMobileNumber"", 
                 ""PaymentStatus"", ""OrderDate"")
                VALUES 
                (@userId, @courseId, @courseName, @courseImage, @price,
                 @paymentMethod, @transactionId, @senderMobileNumber,
                 'Pending', @orderDate)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", order.UserId);
                command.Parameters.AddWithValue("@courseId", order.CourseId);
                command.Parameters.AddWithValue("@courseName", order.CourseName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@courseImage", order.CourseImage ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@price", order.Price);
                command.Parameters.AddWithValue("@paymentMethod", order.PaymentMethod ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@transactionId", order.TransactionId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@senderMobileNumber", order.SenderMobileNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@orderDate", order.OrderDate);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    order.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<CourseOrder> GetUserOrders(int userId)
        {
            var orders = new List<CourseOrder>();
            string query = @"
                SELECT ""Id"", ""UserId"", ""CourseId"", ""CourseName"", ""CourseImage"", 
                       ""Price"", ""PaymentMethod"", ""TransactionId"", ""SenderMobileNumber"",
                       ""PaymentStatus"", ""AdminNote"", ""OrderDate"", ""VerifiedDate"", ""VerifiedBy""
                FROM ""CourseOrders""
                WHERE ""UserId"" = @userId
                ORDER BY ""OrderDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(MapCourseOrder(reader));
                }
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user orders: " + ex.Message);
                return new List<CourseOrder>();
            }
        }

        public List<AdminPaymentViewModel> GetAllOrders(string? statusFilter = null)
        {
            var orders = new List<AdminPaymentViewModel>();
            string query = @"
                SELECT o.""Id"", o.""UserId"", o.""CourseId"", o.""CourseName"", o.""CourseImage"",
                       o.""Price"", o.""PaymentMethod"", o.""TransactionId"", o.""SenderMobileNumber"",
                       o.""PaymentStatus"", o.""AdminNote"", o.""OrderDate"", o.""VerifiedDate"",
                       u.""UserName"", u.""FullName"", u.""Email"", u.""MobileNumber""
                FROM ""CourseOrders"" o
                LEFT JOIN ""Users"" u ON o.""UserId"" = u.""Id""";

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
            {
                query += @" WHERE o.""PaymentStatus"" = @status";
            }

            query += @" ORDER BY o.""OrderDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
                {
                    command.Parameters.AddWithValue("@status", statusFilter);
                }

                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(new AdminPaymentViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        CourseName = reader["CourseName"]?.ToString() ?? "",
                        CourseImage = reader["CourseImage"]?.ToString() ?? "",
                        Price = Convert.ToDecimal(reader["Price"]),
                        PaymentMethod = reader["PaymentMethod"]?.ToString() ?? "",
                        TransactionId = reader["TransactionId"]?.ToString() ?? "",
                        SenderMobileNumber = reader["SenderMobileNumber"]?.ToString() ?? "",
                        PaymentStatus = reader["PaymentStatus"]?.ToString() ?? "Pending",
                        AdminNote = reader["AdminNote"]?.ToString(),
                        OrderDate = reader["OrderDate"] as DateTime? ?? DateTime.UtcNow,
                        VerifiedDate = reader["VerifiedDate"] as DateTime?,
                        UserName = reader["UserName"]?.ToString() ?? "",
                        UserFullName = reader["FullName"]?.ToString() ?? "",
                        UserEmail = reader["Email"]?.ToString() ?? "",
                        UserMobile = reader["MobileNumber"]?.ToString() ?? ""
                    });
                }
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting all orders: " + ex.Message);
                return new List<AdminPaymentViewModel>();
            }
        }

        public CourseOrder? GetOrderById(int orderId)
        {
            string query = @"
                SELECT ""Id"", ""UserId"", ""CourseId"", ""CourseName"", ""CourseImage"", 
                       ""Price"", ""PaymentMethod"", ""TransactionId"", ""SenderMobileNumber"",
                       ""PaymentStatus"", ""AdminNote"", ""OrderDate"", ""VerifiedDate"", ""VerifiedBy""
                FROM ""CourseOrders""
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", orderId);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapCourseOrder(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting order by id: " + ex.Message);
                return null;
            }
        }

        public bool ApproveOrder(int orderId, int adminId, string? adminNote = null)
        {
            string query = @"
                UPDATE ""CourseOrders"" 
                SET ""PaymentStatus"" = 'Approved', 
                    ""VerifiedDate"" = @verifiedDate, 
                    ""VerifiedBy"" = @verifiedBy,
                    ""AdminNote"" = @adminNote
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@verifiedDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@verifiedBy", adminId);
                command.Parameters.AddWithValue("@adminNote", adminNote ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@id", orderId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error approving order: " + ex.Message);
                return false;
            }
        }

        public bool RejectOrder(int orderId, int adminId, string? adminNote = null)
        {
            string query = @"
                UPDATE ""CourseOrders"" 
                SET ""PaymentStatus"" = 'Rejected', 
                    ""VerifiedDate"" = @verifiedDate, 
                    ""VerifiedBy"" = @verifiedBy,
                    ""AdminNote"" = @adminNote
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@verifiedDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@verifiedBy", adminId);
                command.Parameters.AddWithValue("@adminNote", adminNote ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@id", orderId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error rejecting order: " + ex.Message);
                return false;
            }
        }

        public bool IsUserEnrolled(int userId, int courseId)
        {
            string query = @"
                SELECT COUNT(*) FROM ""CourseOrders"" 
                WHERE ""UserId"" = @userId 
                  AND ""CourseId"" = @courseId 
                  AND ""PaymentStatus"" = 'Approved'";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@courseId", courseId);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking enrollment: " + ex.Message);
                return false;
            }
        }

        public List<int> GetUserEnrolledCourseIds(int userId)
        {
            var courseIds = new List<int>();
            string query = @"
                SELECT DISTINCT ""CourseId"" FROM ""CourseOrders"" 
                WHERE ""UserId"" = @userId 
                  AND ""PaymentStatus"" = 'Approved'";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    courseIds.Add(reader.GetInt32(0));
                }
                return courseIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting enrolled courses: " + ex.Message);
                return new List<int>();
            }
        }

        // ============================================================
        // ===== ENROLLED COURSES (MY CLASS) =====
        // ============================================================

        public List<EnrolledCourseViewModel> GetUserEnrolledCourses(int userId)
        {
            var enrolled = new List<EnrolledCourseViewModel>();
            string query = @"
                SELECT DISTINCT ON (""CourseId"")
                       ""CourseId"", ""CourseName"", ""CourseImage"", ""Price"", ""VerifiedDate"", ""OrderDate""
                FROM ""CourseOrders""
                WHERE ""UserId"" = @userId AND ""PaymentStatus"" = 'Approved'
                ORDER BY ""CourseId"", ""OrderDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    enrolled.Add(new EnrolledCourseViewModel
                    {
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        CourseName = reader["CourseName"]?.ToString() ?? "",
                        CourseImage = reader["CourseImage"]?.ToString() ?? "",
                        Price = Convert.ToDecimal(reader["Price"]),
                        EnrolledDate = (reader["VerifiedDate"] as DateTime?)
                                        ?? (reader["OrderDate"] as DateTime?)
                                        ?? DateTime.UtcNow
                    });
                }
                return enrolled;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting enrolled courses: " + ex.Message);
                return new List<EnrolledCourseViewModel>();
            }
        }

        public (int Pending, int Approved, int Rejected, decimal TotalRevenue) GetPaymentStats()
        {
            string query = @"
                SELECT 
                    COUNT(*) FILTER (WHERE ""PaymentStatus"" = 'Pending') as pending,
                    COUNT(*) FILTER (WHERE ""PaymentStatus"" = 'Approved') as approved,
                    COUNT(*) FILTER (WHERE ""PaymentStatus"" = 'Rejected') as rejected,
                    COALESCE(SUM(""Price"") FILTER (WHERE ""PaymentStatus"" = 'Approved'), 0) as revenue
                FROM ""CourseOrders""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        Convert.ToInt32(reader["pending"]),
                        Convert.ToInt32(reader["approved"]),
                        Convert.ToInt32(reader["rejected"]),
                        Convert.ToDecimal(reader["revenue"])
                    );
                }
                return (0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting payment stats: " + ex.Message);
                return (0, 0, 0, 0);
            }
        }

        // ============================================================
        // ===== NOTIFICATION RELATED METHODS =====
        // ============================================================

        public bool CreateNotification(int userId, string title, string message,
            string type = "info", string? link = null, string icon = "fa-bell")
        {
            string query = @"
                INSERT INTO ""Notifications"" 
                (""UserId"", ""Title"", ""Message"", ""Type"", ""Icon"", ""Link"", ""IsRead"", ""CreatedAt"")
                VALUES 
                (@userId, @title, @message, @type, @icon, @link, FALSE, @createdAt)";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@title", title ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@message", message ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@type", type ?? "info");
                command.Parameters.AddWithValue("@icon", icon ?? "fa-bell");
                command.Parameters.AddWithValue("@link", link ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating notification: " + ex.Message);
                return false;
            }
        }

        public List<Notification> GetUserNotifications(int userId, int limit = 20)
        {
            var notifications = new List<Notification>();
            string query = @"
                SELECT ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Icon"", 
                       ""Link"", ""IsRead"", ""CreatedAt""
                FROM ""Notifications""
                WHERE ""UserId"" = @userId
                ORDER BY ""CreatedAt"" DESC
                LIMIT @limit";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@limit", limit);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    notifications.Add(new Notification
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Title = reader["Title"]?.ToString() ?? "",
                        Message = reader["Message"]?.ToString() ?? "",
                        Type = reader["Type"]?.ToString() ?? "info",
                        Icon = reader["Icon"]?.ToString() ?? "fa-bell",
                        Link = reader["Link"]?.ToString(),
                        IsRead = reader["IsRead"] as bool? ?? false,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow
                    });
                }
                return notifications;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting notifications: " + ex.Message);
                return new List<Notification>();
            }
        }

        public int GetUnreadNotificationCount(int userId)
        {
            string query = @"SELECT COUNT(*) FROM ""Notifications"" 
                             WHERE ""UserId"" = @userId AND ""IsRead"" = FALSE";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting unread count: " + ex.Message);
                return 0;
            }
        }

        public bool MarkNotificationAsRead(int notificationId, int userId)
        {
            string query = @"UPDATE ""Notifications"" 
                             SET ""IsRead"" = TRUE 
                             WHERE ""Id"" = @id AND ""UserId"" = @userId";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", notificationId);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error marking as read: " + ex.Message);
                return false;
            }
        }

        public bool MarkAllNotificationsAsRead(int userId)
        {
            string query = @"UPDATE ""Notifications"" 
                             SET ""IsRead"" = TRUE 
                             WHERE ""UserId"" = @userId AND ""IsRead"" = FALSE";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return command.ExecuteNonQuery() >= 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error marking all as read: " + ex.Message);
                return false;
            }
        }

        public bool ClearUserNotifications(int userId)
        {
            string query = @"DELETE FROM ""Notifications"" WHERE ""UserId"" = @userId";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return command.ExecuteNonQuery() >= 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing notifications: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== NOTICE / NEWS / ANNOUNCEMENT METHODS =====
        // ============================================================

        public bool CreateNotice(Notice notice, out string? errorMessage)
        {
            errorMessage = null;

            // 🔧 FIX: Category = "News" হলে IsAnnouncement force FALSE
            if (string.Equals(notice.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                notice.IsAnnouncement = false;
                Console.WriteLine("🔧 DB FIX: Category=News → IsAnnouncement forced to FALSE");
            }

            if (notice.PublishedDate == default(DateTime) ||
                notice.PublishedDate.Year < 2000 ||
                notice.PublishedDate.Year > 2100)
            {
                notice.PublishedDate = DateTime.Now;
                Console.WriteLine("🔧 DB FIX: PublishedDate was invalid → set to NOW");
            }

            string query = @"
                INSERT INTO ""Notices"" 
                (""Title"", ""Content"", ""Category"", ""ImageUrl"", 
                 ""PublishedDate"", ""IsAnnouncement"", ""IsActive"", ""CreatedAt"")
                VALUES 
                (@title, @content, @category, @imageUrl,
                 @publishedDate, @isAnnouncement, @isActive, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@title", notice.Title ?? "");
                command.Parameters.AddWithValue("@content", notice.Content ?? "");
                command.Parameters.AddWithValue("@category", string.IsNullOrWhiteSpace(notice.Category) ? "News" : notice.Category);
                command.Parameters.AddWithValue("@imageUrl", notice.ImageUrl ?? "");
                command.Parameters.AddWithValue("@publishedDate", notice.PublishedDate);
                command.Parameters.AddWithValue("@isAnnouncement", notice.IsAnnouncement);
                command.Parameters.AddWithValue("@isActive", notice.IsActive);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    notice.Id = newId;
                    Console.WriteLine($"✅ Notice saved: Id={newId}, Category={notice.Category}, IsAnnouncement={notice.IsAnnouncement}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<Notice> GetAllNotices()
        {
            var notices = new List<Notice>();
            string query = @"SELECT * FROM ""Notices"" WHERE ""IsActive"" = TRUE ORDER BY ""PublishedDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    notices.Add(MapNotice(reader));
                }

                Console.WriteLine($"GetAllNotices: {notices.Count} notices found");
                return notices;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting all notices: " + ex.Message);
                return new List<Notice>();
            }
        }

        public Notice? GetNoticeById(int id)
        {
            string query = @"SELECT * FROM ""Notices"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapNotice(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting notice by id: " + ex.Message);
                return null;
            }
        }

        public bool UpdateNotice(Notice notice, out string? errorMessage)
        {
            errorMessage = null;

            // 🔧 FIX: Category = "News" হলে IsAnnouncement force FALSE
            if (string.Equals(notice.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                notice.IsAnnouncement = false;
                Console.WriteLine("🔧 DB UPDATE FIX: Category=News → IsAnnouncement forced to FALSE");
            }

            if (notice.PublishedDate == default(DateTime) ||
                notice.PublishedDate.Year < 2000 ||
                notice.PublishedDate.Year > 2100)
            {
                notice.PublishedDate = DateTime.Now;
            }

            string query = @"
                UPDATE ""Notices"" SET 
                    ""Title"" = @title,
                    ""Content"" = @content,
                    ""Category"" = @category,
                    ""ImageUrl"" = @imageUrl,
                    ""PublishedDate"" = @publishedDate,
                    ""IsAnnouncement"" = @isAnnouncement,
                    ""IsActive"" = @isActive,
                    ""UpdatedAt"" = @updatedAt
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", notice.Id);
                command.Parameters.AddWithValue("@title", notice.Title ?? "");
                command.Parameters.AddWithValue("@content", notice.Content ?? "");
                command.Parameters.AddWithValue("@category", string.IsNullOrWhiteSpace(notice.Category) ? "News" : notice.Category);
                command.Parameters.AddWithValue("@imageUrl", notice.ImageUrl ?? "");
                command.Parameters.AddWithValue("@publishedDate", notice.PublishedDate);
                command.Parameters.AddWithValue("@isAnnouncement", notice.IsAnnouncement);
                command.Parameters.AddWithValue("@isActive", notice.IsActive);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool DeleteNotice(int id)
        {
            string query = @"DELETE FROM ""Notices"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting notice: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== QUIZ RELATED METHODS =====
        // ============================================================

        public bool CreateModuleQuiz(ModuleQuiz quiz, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""ModuleQuizzes"" 
                (""CourseId"", ""ModuleNumber"", ""Title"", ""Description"", 
                 ""PassingScore"", ""TimeLimitMinutes"", ""IsPublished"", ""CreatedAt"")
                VALUES 
                (@courseId, @moduleNumber, @title, @description,
                 @passingScore, @timeLimit, @isPublished, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@courseId", quiz.CourseId);
                command.Parameters.AddWithValue("@moduleNumber", quiz.ModuleNumber);
                command.Parameters.AddWithValue("@title", quiz.Title ?? "");
                command.Parameters.AddWithValue("@description", quiz.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@passingScore", quiz.PassingScore);
                command.Parameters.AddWithValue("@timeLimit", quiz.TimeLimitMinutes);
                command.Parameters.AddWithValue("@isPublished", quiz.IsPublished);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    quiz.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<ModuleQuiz> GetQuizzesByCourse(int courseId)
        {
            var quizzes = new List<ModuleQuiz>();
            string query = @"
                SELECT q.""Id"", q.""CourseId"", q.""ModuleNumber"", q.""Title"", q.""Description"",
                       q.""PassingScore"", q.""TimeLimitMinutes"", q.""IsPublished"", q.""CreatedAt"", q.""UpdatedAt"",
                       (SELECT COUNT(*) FROM ""QuizQuestions"" WHERE ""QuizId"" = q.""Id"") as QuestionCount,
                       COALESCE((SELECT SUM(""Marks"") FROM ""QuizQuestions"" WHERE ""QuizId"" = q.""Id""), 0) as TotalMarks
                FROM ""ModuleQuizzes"" q
                WHERE q.""CourseId"" = @courseId
                ORDER BY q.""ModuleNumber""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@courseId", courseId);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    quizzes.Add(new ModuleQuiz
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        ModuleNumber = Convert.ToInt32(reader["ModuleNumber"]),
                        Title = reader["Title"]?.ToString() ?? "",
                        Description = reader["Description"]?.ToString(),
                        PassingScore = Convert.ToInt32(reader["PassingScore"]),
                        TimeLimitMinutes = Convert.ToInt32(reader["TimeLimitMinutes"]),
                        IsPublished = reader["IsPublished"] as bool? ?? false,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UpdatedAt = reader["UpdatedAt"] as DateTime?,
                        QuestionCount = Convert.ToInt32(reader["QuestionCount"]),
                        TotalMarks = Convert.ToInt32(reader["TotalMarks"])
                    });
                }
                return quizzes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting quizzes: " + ex.Message);
                return new List<ModuleQuiz>();
            }
        }

        public ModuleQuiz? GetQuizById(int quizId, bool includeQuestions = false)
        {
            string query = @"SELECT * FROM ""ModuleQuizzes"" WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", quizId);
                connection.Open();

                ModuleQuiz? quiz = null;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        quiz = new ModuleQuiz
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                            ModuleNumber = Convert.ToInt32(reader["ModuleNumber"]),
                            Title = reader["Title"]?.ToString() ?? "",
                            Description = reader["Description"]?.ToString(),
                            PassingScore = Convert.ToInt32(reader["PassingScore"]),
                            TimeLimitMinutes = Convert.ToInt32(reader["TimeLimitMinutes"]),
                            IsPublished = reader["IsPublished"] as bool? ?? false,
                            CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                            UpdatedAt = reader["UpdatedAt"] as DateTime?
                        };
                    }
                }

                if (quiz != null && includeQuestions)
                {
                    quiz.Questions = GetQuizQuestions(quizId);
                    quiz.QuestionCount = quiz.Questions.Count;
                    quiz.TotalMarks = quiz.Questions.Sum(q => q.Marks);
                }

                return quiz;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting quiz by id: " + ex.Message);
                return null;
            }
        }

        public bool UpdateModuleQuiz(ModuleQuiz quiz, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                UPDATE ""ModuleQuizzes"" SET
                    ""ModuleNumber"" = @moduleNumber,
                    ""Title"" = @title,
                    ""Description"" = @description,
                    ""PassingScore"" = @passingScore,
                    ""TimeLimitMinutes"" = @timeLimit,
                    ""IsPublished"" = @isPublished,
                    ""UpdatedAt"" = @updatedAt
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", quiz.Id);
                command.Parameters.AddWithValue("@moduleNumber", quiz.ModuleNumber);
                command.Parameters.AddWithValue("@title", quiz.Title ?? "");
                command.Parameters.AddWithValue("@description", quiz.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@passingScore", quiz.PassingScore);
                command.Parameters.AddWithValue("@timeLimit", quiz.TimeLimitMinutes);
                command.Parameters.AddWithValue("@isPublished", quiz.IsPublished);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool DeleteModuleQuiz(int quizId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var cmd = new NpgsqlCommand(@"DELETE FROM ""ModuleQuizzes"" WHERE ""Id"" = @id", connection);
                cmd.Parameters.AddWithValue("@id", quizId);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting quiz: " + ex.Message);
                return false;
            }
        }

        public bool ToggleQuizPublish(int quizId, bool isPublished)
        {
            string query = @"UPDATE ""ModuleQuizzes"" SET ""IsPublished"" = @isPub, ""UpdatedAt"" = @upd WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@isPub", isPublished);
                command.Parameters.AddWithValue("@upd", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", quizId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public List<QuizQuestionItem> GetQuizQuestions(int quizId)
        {
            var questions = new List<QuizQuestionItem>();
            string query = @"SELECT * FROM ""QuizQuestions"" WHERE ""QuizId"" = @quizId ORDER BY ""QuestionOrder"", ""Id""";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@quizId", quizId);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    questions.Add(new QuizQuestionItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        QuizId = reader.GetInt32(reader.GetOrdinal("QuizId")),
                        QuestionText = reader["QuestionText"]?.ToString() ?? "",
                        OptionA = reader["OptionA"]?.ToString() ?? "",
                        OptionB = reader["OptionB"]?.ToString() ?? "",
                        OptionC = reader["OptionC"]?.ToString() ?? "",
                        OptionD = reader["OptionD"]?.ToString() ?? "",
                        CorrectOption = reader["CorrectOption"]?.ToString() ?? "A",
                        Marks = Convert.ToInt32(reader["Marks"]),
                        QuestionOrder = Convert.ToInt32(reader["QuestionOrder"])
                    });
                }
                return questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting quiz questions: " + ex.Message);
                return new List<QuizQuestionItem>();
            }
        }

        public bool AddQuizQuestion(QuizQuestionItem q, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""QuizQuestions"" 
                (""QuizId"", ""QuestionText"", ""OptionA"", ""OptionB"", ""OptionC"", ""OptionD"",
                 ""CorrectOption"", ""Marks"", ""QuestionOrder"", ""CreatedAt"")
                VALUES 
                (@quizId, @qText, @a, @b, @c, @d, @correct, @marks, @order, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@quizId", q.QuizId);
                command.Parameters.AddWithValue("@qText", q.QuestionText ?? "");
                command.Parameters.AddWithValue("@a", q.OptionA ?? "");
                command.Parameters.AddWithValue("@b", q.OptionB ?? "");
                command.Parameters.AddWithValue("@c", q.OptionC ?? "");
                command.Parameters.AddWithValue("@d", q.OptionD ?? "");
                command.Parameters.AddWithValue("@correct", q.CorrectOption ?? "A");
                command.Parameters.AddWithValue("@marks", q.Marks);
                command.Parameters.AddWithValue("@order", q.QuestionOrder);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    q.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool DeleteQuizQuestion(int questionId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var cmd = new NpgsqlCommand(@"DELETE FROM ""QuizQuestions"" WHERE ""Id"" = @id", connection);
                cmd.Parameters.AddWithValue("@id", questionId);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public bool SaveQuizAttempt(QuizAttempt attempt, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""QuizAttempts"" 
                (""UserId"", ""QuizId"", ""CourseId"", ""Score"", ""TotalMarks"", 
                 ""Percentage"", ""Passed"", ""AttemptedAt"")
                VALUES 
                (@userId, @quizId, @courseId, @score, @total,
                 @pct, @passed, @attemptedAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", attempt.UserId);
                command.Parameters.AddWithValue("@quizId", attempt.QuizId);
                command.Parameters.AddWithValue("@courseId", attempt.CourseId);
                command.Parameters.AddWithValue("@score", attempt.Score);
                command.Parameters.AddWithValue("@total", attempt.TotalMarks);
                command.Parameters.AddWithValue("@pct", attempt.Percentage);
                command.Parameters.AddWithValue("@passed", attempt.Passed);
                command.Parameters.AddWithValue("@attemptedAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    attempt.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public QuizAttempt? GetBestQuizAttempt(int userId, int quizId)
        {
            string query = @"
                SELECT * FROM ""QuizAttempts"" 
                WHERE ""UserId"" = @userId AND ""QuizId"" = @quizId
                ORDER BY ""Percentage"" DESC, ""AttemptedAt"" DESC
                LIMIT 1";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@quizId", quizId);
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new QuizAttempt
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        QuizId = reader.GetInt32(reader.GetOrdinal("QuizId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        Score = Convert.ToInt32(reader["Score"]),
                        TotalMarks = Convert.ToInt32(reader["TotalMarks"]),
                        Percentage = Convert.ToDecimal(reader["Percentage"]),
                        Passed = reader["Passed"] as bool? ?? false,
                        AttemptedAt = reader["AttemptedAt"] as DateTime? ?? DateTime.UtcNow
                    };
                }
                return null;
            }
            catch { return null; }
        }

        public List<QuizAttempt> GetUserQuizAttempts(int userId, int courseId)
        {
            var attempts = new List<QuizAttempt>();
            string query = @"
                SELECT a.*, q.""Title"" as QuizTitle, q.""ModuleNumber""
                FROM ""QuizAttempts"" a
                LEFT JOIN ""ModuleQuizzes"" q ON a.""QuizId"" = q.""Id""
                WHERE a.""UserId"" = @userId AND a.""CourseId"" = @courseId
                ORDER BY a.""AttemptedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@courseId", courseId);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    attempts.Add(new QuizAttempt
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        QuizId = reader.GetInt32(reader.GetOrdinal("QuizId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        Score = Convert.ToInt32(reader["Score"]),
                        TotalMarks = Convert.ToInt32(reader["TotalMarks"]),
                        Percentage = Convert.ToDecimal(reader["Percentage"]),
                        Passed = reader["Passed"] as bool? ?? false,
                        AttemptedAt = reader["AttemptedAt"] as DateTime? ?? DateTime.UtcNow,
                        QuizTitle = reader["QuizTitle"]?.ToString() ?? "",
                        ModuleNumber = reader["ModuleNumber"] != DBNull.Value ? Convert.ToInt32(reader["ModuleNumber"]) : 0
                    });
                }
                return attempts;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user quiz attempts: " + ex.Message);
                return new List<QuizAttempt>();
            }
        }

        public QuizAttempt? GetQuizAttemptById(int attemptId, int userId)
        {
            string query = @"SELECT * FROM ""QuizAttempts"" WHERE ""Id"" = @id AND ""UserId"" = @u";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", attemptId);
                command.Parameters.AddWithValue("@u", userId);
                connection.Open();
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new QuizAttempt
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        QuizId = reader.GetInt32(reader.GetOrdinal("QuizId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        Score = Convert.ToInt32(reader["Score"]),
                        TotalMarks = Convert.ToInt32(reader["TotalMarks"]),
                        Percentage = Convert.ToDecimal(reader["Percentage"]),
                        Passed = reader["Passed"] as bool? ?? false,
                        AttemptedAt = reader["AttemptedAt"] as DateTime? ?? DateTime.UtcNow
                    };
                }
            }
            catch { }
            return null;
        }

        // ===== Lesson Completion Tracking =====
        public bool MarkLessonCompleted(int userId, int courseId, int lessonId, int moduleNumber)
        {
            string query = @"
                INSERT INTO ""LessonCompletions"" (""UserId"", ""CourseId"", ""LessonId"", ""ModuleNumber"", ""CompletedAt"")
                VALUES (@userId, @courseId, @lessonId, @moduleNumber, @completedAt)
                ON CONFLICT (""UserId"", ""LessonId"") DO NOTHING";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@courseId", courseId);
                command.Parameters.AddWithValue("@lessonId", lessonId);
                command.Parameters.AddWithValue("@moduleNumber", moduleNumber);
                command.Parameters.AddWithValue("@completedAt", DateTime.UtcNow);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error marking lesson completed: " + ex.Message);
                return false;
            }
        }

        public HashSet<int> GetCompletedLessonIds(int userId, int courseId)
        {
            var ids = new HashSet<int>();
            string query = @"SELECT ""LessonId"" FROM ""LessonCompletions"" WHERE ""UserId"" = @u AND ""CourseId"" = @c";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@u", userId);
                command.Parameters.AddWithValue("@c", courseId);
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read()) ids.Add(reader.GetInt32(0));
                return ids;
            }
            catch { return ids; }
        }

        public Dictionary<int, (int Total, int Completed)> GetModuleProgress(int userId, int courseId)
        {
            var result = new Dictionary<int, (int Total, int Completed)>();

            var totalByModule = new Dictionary<int, int>();
            string totalQuery = @"SELECT ""ModuleNumber"", COUNT(*) FROM ""CourseLessons"" WHERE ""CourseId"" = @c GROUP BY ""ModuleNumber""";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(totalQuery, connection);
                command.Parameters.AddWithValue("@c", courseId);
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    totalByModule[Convert.ToInt32(reader[0])] = Convert.ToInt32(reader[1]);
                }
            }
            catch { }

            var completedByModule = new Dictionary<int, int>();
            string completedQuery = @"
                SELECT ""ModuleNumber"", COUNT(*) 
                FROM ""LessonCompletions"" 
                WHERE ""UserId"" = @u AND ""CourseId"" = @c 
                GROUP BY ""ModuleNumber""";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(completedQuery, connection);
                command.Parameters.AddWithValue("@u", userId);
                command.Parameters.AddWithValue("@c", courseId);
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    completedByModule[Convert.ToInt32(reader[0])] = Convert.ToInt32(reader[1]);
                }
            }
            catch { }

            foreach (var kvp in totalByModule)
            {
                int module = kvp.Key;
                int total = kvp.Value;
                int completed = completedByModule.TryGetValue(module, out var c) ? c : 0;
                result[module] = (total, completed);
            }

            return result;
        }

        // ===== Milestone Assignment Methods =====
        public bool CreateMilestoneAssignment(MilestoneAssignment a, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""MilestoneAssignments"" 
                (""CourseId"", ""MilestoneNumber"", ""Title"", ""Description"", ""Instructions"",
                 ""TotalMarks"", ""DueDays"", ""IsPublished"", ""CreatedAt"")
                VALUES 
                (@courseId, @milestoneNum, @title, @desc, @instr,
                 @marks, @days, @isPub, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@courseId", a.CourseId);
                command.Parameters.AddWithValue("@milestoneNum", a.MilestoneNumber);
                command.Parameters.AddWithValue("@title", a.Title ?? "");
                command.Parameters.AddWithValue("@desc", a.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@instr", a.Instructions ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@marks", a.TotalMarks);
                command.Parameters.AddWithValue("@days", a.DueDays);
                command.Parameters.AddWithValue("@isPub", a.IsPublished);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    a.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<MilestoneAssignment> GetAssignmentsByCourse(int courseId)
        {
            var list = new List<MilestoneAssignment>();
            string query = @"SELECT * FROM ""MilestoneAssignments"" WHERE ""CourseId"" = @c ORDER BY ""MilestoneNumber""";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@c", courseId);
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapAssignment(reader));
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting assignments: " + ex.Message);
                return list;
            }
        }

        public MilestoneAssignment? GetAssignmentById(int id)
        {
            string query = @"SELECT * FROM ""MilestoneAssignments"" WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using var reader = command.ExecuteReader();
                if (reader.Read()) return MapAssignment(reader);
                return null;
            }
            catch { return null; }
        }

        public bool UpdateMilestoneAssignment(MilestoneAssignment a, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                UPDATE ""MilestoneAssignments"" SET
                    ""MilestoneNumber"" = @milestoneNum,
                    ""Title"" = @title,
                    ""Description"" = @desc,
                    ""Instructions"" = @instr,
                    ""TotalMarks"" = @marks,
                    ""DueDays"" = @days,
                    ""IsPublished"" = @isPub,
                    ""UpdatedAt"" = @upd
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", a.Id);
                command.Parameters.AddWithValue("@milestoneNum", a.MilestoneNumber);
                command.Parameters.AddWithValue("@title", a.Title ?? "");
                command.Parameters.AddWithValue("@desc", a.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@instr", a.Instructions ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@marks", a.TotalMarks);
                command.Parameters.AddWithValue("@days", a.DueDays);
                command.Parameters.AddWithValue("@isPub", a.IsPublished);
                command.Parameters.AddWithValue("@upd", DateTime.UtcNow);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool DeleteMilestoneAssignment(int id)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var cmd = new NpgsqlCommand(@"DELETE FROM ""MilestoneAssignments"" WHERE ""Id"" = @id", connection);
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public bool ToggleAssignmentPublish(int id, bool isPublished)
        {
            string query = @"UPDATE ""MilestoneAssignments"" SET ""IsPublished"" = @p, ""UpdatedAt"" = @u WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@p", isPublished);
                command.Parameters.AddWithValue("@u", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public int GetModulesPerMilestone(int courseId)
        {
            string query = @"SELECT ""ModulesPerMilestone"" FROM ""CourseMilestoneConfig"" WHERE ""CourseId"" = @c";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@c", courseId);
                connection.Open();
                var res = command.ExecuteScalar();
                if (res != null && res != DBNull.Value) return Convert.ToInt32(res);
                return 4;
            }
            catch { return 4; }
        }

        public bool SetModulesPerMilestone(int courseId, int modulesPerMilestone)
        {
            string query = @"
                INSERT INTO ""CourseMilestoneConfig"" (""CourseId"", ""ModulesPerMilestone"", ""CreatedAt"")
                VALUES (@c, @m, @t)
                ON CONFLICT (""CourseId"") DO UPDATE SET ""ModulesPerMilestone"" = @m";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@c", courseId);
                command.Parameters.AddWithValue("@m", modulesPerMilestone);
                command.Parameters.AddWithValue("@t", DateTime.UtcNow);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        // ============================================================
        // ===== ASSIGNMENT SUBMISSION METHODS =====
        // ============================================================

        public bool CreateAssignmentSubmission(AssignmentSubmission submission, out string? errorMessage)
        {
            errorMessage = null;

            try
            {
                // Late check
                var (isLate, daysLate, latePointsCost, _) =
                    CheckLateSubmission(submission.AssignmentId, DateTime.UtcNow);

                int pointsDeducted = 0;

                if (isLate)
                {
                    if (!SpendPoints(submission.UserId, latePointsCost,
                        "Spent_Late",
                        $"assignment_{submission.AssignmentId}_late",
                        $"Late submission fee ({daysLate} days late)",
                        out string? spendError))
                    {
                        errorMessage = $"Late submission requires {latePointsCost} points. {spendError}";
                        return false;
                    }
                    pointsDeducted = latePointsCost;
                }

                string query = @"
                    INSERT INTO ""AssignmentSubmissions"" 
                    (""AssignmentId"", ""UserId"", ""CourseId"", ""DriveLink"", ""Note"", 
                     ""Status"", ""SubmittedAt"", ""IsLateSubmission"", ""LatePointsCharged"")
                    VALUES 
                    (@assignmentId, @userId, @courseId, @driveLink, @note, 
                     'Submitted', @submittedAt, @isLate, @latePoints)
                    ON CONFLICT (""AssignmentId"", ""UserId"") 
                    DO UPDATE SET 
                        ""DriveLink"" = EXCLUDED.""DriveLink"",
                        ""Note"" = EXCLUDED.""Note"",
                        ""SubmittedAt"" = EXCLUDED.""SubmittedAt"",
                        ""Status"" = 'Resubmitted',
                        ""Marks"" = NULL,
                        ""Feedback"" = NULL,
                        ""GradedAt"" = NULL,
                        ""GradedBy"" = NULL,
                        ""IsLateSubmission"" = EXCLUDED.""IsLateSubmission"",
                        ""LatePointsCharged"" = ""AssignmentSubmissions"".""LatePointsCharged"" + EXCLUDED.""LatePointsCharged""
                    RETURNING ""Id""";

                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@assignmentId", submission.AssignmentId);
                command.Parameters.AddWithValue("@userId", submission.UserId);
                command.Parameters.AddWithValue("@courseId", submission.CourseId);
                command.Parameters.AddWithValue("@driveLink", submission.DriveLink ?? "");
                command.Parameters.AddWithValue("@note", submission.Note ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@submittedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@isLate", isLate);
                command.Parameters.AddWithValue("@latePoints", pointsDeducted);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    submission.Id = newId;
                    submission.IsLateSubmission = isLate;
                    submission.LatePointsCharged = pointsDeducted;
                    Console.WriteLine($"✅ Submission saved: Id={newId}, IsLate={isLate}, PointsDeducted={pointsDeducted}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Console.WriteLine("❌ Error CreateAssignmentSubmission: " + ex.Message);
                return false;
            }
        }

        public AssignmentSubmission? GetUserSubmission(int userId, int assignmentId)
        {
            string query = @"
                SELECT s.*, a.""Title"" AS AssignmentTitle, a.""MilestoneNumber"", 
                       a.""TotalMarks"", a.""DueDays"", a.""CreatedAt"" AS AssignmentCreatedAt
                FROM ""AssignmentSubmissions"" s
                LEFT JOIN ""MilestoneAssignments"" a ON s.""AssignmentId"" = a.""Id""
                WHERE s.""UserId"" = @userId AND s.""AssignmentId"" = @assignmentId
                LIMIT 1";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@assignmentId", assignmentId);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var sub = new AssignmentSubmission
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        DriveLink = reader["DriveLink"]?.ToString() ?? "",
                        Note = reader["Note"]?.ToString(),
                        SubmittedAt = reader["SubmittedAt"] as DateTime? ?? DateTime.UtcNow,
                        Marks = reader["Marks"] != DBNull.Value ? Convert.ToInt32(reader["Marks"]) : (int?)null,
                        Feedback = reader["Feedback"]?.ToString(),
                        GradedAt = reader["GradedAt"] as DateTime?,
                        GradedBy = reader["GradedBy"] != DBNull.Value ? Convert.ToInt32(reader["GradedBy"]) : (int?)null,
                        Status = reader["Status"]?.ToString() ?? "Submitted",
                        AssignmentTitle = reader["AssignmentTitle"]?.ToString() ?? "",
                        MilestoneNumber = reader["MilestoneNumber"] != DBNull.Value ? Convert.ToInt32(reader["MilestoneNumber"]) : 0,
                        TotalMarks = reader["TotalMarks"] != DBNull.Value ? Convert.ToInt32(reader["TotalMarks"]) : 0
                    };

                    try { sub.ResubmitCount = Convert.ToInt32(reader["ResubmitCount"]); } catch { }
                    try { sub.RecheckCount = Convert.ToInt32(reader["RecheckCount"]); } catch { }
                    try { sub.PointsSpent = Convert.ToInt32(reader["PointsSpent"]); } catch { }
                    try { sub.FirstMarks = reader["FirstMarks"] != DBNull.Value ? Convert.ToInt32(reader["FirstMarks"]) : (int?)null; } catch { }
                    try { sub.IsLateSubmission = reader["IsLateSubmission"] as bool? ?? false; } catch { }
                    try { sub.LatePointsCharged = Convert.ToInt32(reader["LatePointsCharged"]); } catch { }

                    try
                    {
                        var assignmentCreatedAt = reader["AssignmentCreatedAt"] as DateTime?;
                        var dueDays = reader["DueDays"] != DBNull.Value ? Convert.ToInt32(reader["DueDays"]) : 0;
                        if (assignmentCreatedAt.HasValue && dueDays > 0)
                        {
                            sub.DueDate = assignmentCreatedAt.Value.AddDays(dueDays);
                        }
                    }
                    catch { }

                    return sub;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetUserSubmission: " + ex.Message);
                return null;
            }
        }

        public List<AdminSubmissionViewModel> GetAllSubmissions(int? courseId = null, int? assignmentId = null, string? status = null)
        {
            var list = new List<AdminSubmissionViewModel>();

            string query = @"
                SELECT 
                    s.""Id"", s.""AssignmentId"", s.""UserId"", s.""CourseId"",
                    s.""DriveLink"", s.""Note"", s.""SubmittedAt"",
                    s.""Marks"", s.""Feedback"", s.""GradedAt"", s.""Status"",
                    a.""Title"" AS AssignmentTitle, a.""MilestoneNumber"", a.""TotalMarks"",
                    c.""Title"" AS CourseName,
                    u.""FullName"" AS UserFullName, u.""Email"" AS UserEmail, u.""UserName"" AS UserName
                FROM ""AssignmentSubmissions"" s
                LEFT JOIN ""MilestoneAssignments"" a ON s.""AssignmentId"" = a.""Id""
                LEFT JOIN ""Courses"" c ON s.""CourseId"" = c.""Id""
                LEFT JOIN ""Users"" u ON s.""UserId"" = u.""Id""
                WHERE 1=1";

            if (courseId.HasValue && courseId.Value > 0)
                query += @" AND s.""CourseId"" = @courseId";

            if (assignmentId.HasValue && assignmentId.Value > 0)
                query += @" AND s.""AssignmentId"" = @assignmentId";

            if (!string.IsNullOrEmpty(status) && status != "all")
                query += @" AND s.""Status"" = @status";

            query += @" ORDER BY s.""SubmittedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                if (courseId.HasValue && courseId.Value > 0)
                    command.Parameters.AddWithValue("@courseId", courseId.Value);

                if (assignmentId.HasValue && assignmentId.Value > 0)
                    command.Parameters.AddWithValue("@assignmentId", assignmentId.Value);

                if (!string.IsNullOrEmpty(status) && status != "all")
                    command.Parameters.AddWithValue("@status", status);

                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var vm = new AdminSubmissionViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        DriveLink = reader["DriveLink"]?.ToString() ?? "",
                        Note = reader["Note"]?.ToString(),
                        SubmittedAt = reader["SubmittedAt"] as DateTime? ?? DateTime.UtcNow,
                        Marks = reader["Marks"] != DBNull.Value ? Convert.ToInt32(reader["Marks"]) : (int?)null,
                        Feedback = reader["Feedback"]?.ToString(),
                        GradedAt = reader["GradedAt"] as DateTime?,
                        Status = reader["Status"]?.ToString() ?? "Submitted",
                        AssignmentTitle = reader["AssignmentTitle"]?.ToString() ?? "",
                        MilestoneNumber = reader["MilestoneNumber"] != DBNull.Value ? Convert.ToInt32(reader["MilestoneNumber"]) : 0,
                        TotalMarks = reader["TotalMarks"] != DBNull.Value ? Convert.ToInt32(reader["TotalMarks"]) : 0,
                        CourseName = reader["CourseName"]?.ToString() ?? "",
                        UserFullName = reader["UserFullName"]?.ToString() ?? "",
                        UserEmail = reader["UserEmail"]?.ToString() ?? "",
                        UserName = reader["UserName"]?.ToString() ?? ""
                    };

                    try { vm.ResubmitCount = Convert.ToInt32(reader["ResubmitCount"]); } catch { }
                    try { vm.RecheckCount = Convert.ToInt32(reader["RecheckCount"]); } catch { }
                    try { vm.PointsSpent = Convert.ToInt32(reader["PointsSpent"]); } catch { }
                    try { vm.FirstMarks = reader["FirstMarks"] != DBNull.Value ? Convert.ToInt32(reader["FirstMarks"]) : (int?)null; } catch { }
                    try { vm.IsLateSubmission = reader["IsLateSubmission"] as bool? ?? false; } catch { }
                    try { vm.LatePointsCharged = Convert.ToInt32(reader["LatePointsCharged"]); } catch { }

                    list.Add(vm);
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetAllSubmissions: " + ex.Message);
                return list;
            }
        }

        public AssignmentSubmission? GetSubmissionById(int submissionId)
        {
            string query = @"
                SELECT s.*, a.""Title"" AS AssignmentTitle, a.""MilestoneNumber"", 
                       a.""TotalMarks"", a.""DueDays"", a.""CreatedAt"" AS AssignmentCreatedAt,
                       c.""Title"" AS CourseName,
                       u.""FullName"" AS UserFullName, u.""Email"" AS UserEmail, u.""UserName"" AS UserName
                FROM ""AssignmentSubmissions"" s
                LEFT JOIN ""MilestoneAssignments"" a ON s.""AssignmentId"" = a.""Id""
                LEFT JOIN ""Courses"" c ON s.""CourseId"" = c.""Id""
                LEFT JOIN ""Users"" u ON s.""UserId"" = u.""Id""
                WHERE s.""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", submissionId);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var sub = new AssignmentSubmission
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        DriveLink = reader["DriveLink"]?.ToString() ?? "",
                        Note = reader["Note"]?.ToString(),
                        SubmittedAt = reader["SubmittedAt"] as DateTime? ?? DateTime.UtcNow,
                        Marks = reader["Marks"] != DBNull.Value ? Convert.ToInt32(reader["Marks"]) : (int?)null,
                        Feedback = reader["Feedback"]?.ToString(),
                        GradedAt = reader["GradedAt"] as DateTime?,
                        GradedBy = reader["GradedBy"] != DBNull.Value ? Convert.ToInt32(reader["GradedBy"]) : (int?)null,
                        Status = reader["Status"]?.ToString() ?? "Submitted",
                        AssignmentTitle = reader["AssignmentTitle"]?.ToString() ?? "",
                        MilestoneNumber = reader["MilestoneNumber"] != DBNull.Value ? Convert.ToInt32(reader["MilestoneNumber"]) : 0,
                        TotalMarks = reader["TotalMarks"] != DBNull.Value ? Convert.ToInt32(reader["TotalMarks"]) : 0,
                        CourseName = reader["CourseName"]?.ToString() ?? "",
                        UserFullName = reader["UserFullName"]?.ToString() ?? "",
                        UserEmail = reader["UserEmail"]?.ToString() ?? "",
                        UserName = reader["UserName"]?.ToString() ?? ""
                    };

                    try { sub.ResubmitCount = Convert.ToInt32(reader["ResubmitCount"]); } catch { }
                    try { sub.RecheckCount = Convert.ToInt32(reader["RecheckCount"]); } catch { }
                    try { sub.PointsSpent = Convert.ToInt32(reader["PointsSpent"]); } catch { }
                    try { sub.FirstMarks = reader["FirstMarks"] != DBNull.Value ? Convert.ToInt32(reader["FirstMarks"]) : (int?)null; } catch { }
                    try { sub.IsLateSubmission = reader["IsLateSubmission"] as bool? ?? false; } catch { }
                    try { sub.LatePointsCharged = Convert.ToInt32(reader["LatePointsCharged"]); } catch { }

                    try
                    {
                        var assignmentCreatedAt = reader["AssignmentCreatedAt"] as DateTime?;
                        var dueDays = reader["DueDays"] != DBNull.Value ? Convert.ToInt32(reader["DueDays"]) : 0;
                        if (assignmentCreatedAt.HasValue && dueDays > 0)
                        {
                            sub.DueDate = assignmentCreatedAt.Value.AddDays(dueDays);
                        }
                    }
                    catch { }

                    return sub;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetSubmissionById: " + ex.Message);
                return null;
            }
        }

        public bool GradeSubmission(int submissionId, int marks, string? feedback, int adminId)
        {
            string query = @"
                UPDATE ""AssignmentSubmissions"" SET
                    ""Marks"" = @marks,
                    ""Feedback"" = @feedback,
                    ""GradedAt"" = @gradedAt,
                    ""GradedBy"" = @gradedBy,
                    ""Status"" = 'Graded'
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", submissionId);
                command.Parameters.AddWithValue("@marks", marks);
                command.Parameters.AddWithValue("@feedback", feedback ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gradedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@gradedBy", adminId);

                connection.Open();
                bool ok = command.ExecuteNonQuery() > 0;
                Console.WriteLine($"✅ GradeSubmission: submissionId={submissionId}, marks={marks}, ok={ok}");
                return ok;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GradeSubmission: " + ex.Message);
                return false;
            }
        }

        public List<AssignmentSubmission> GetUserSubmissionsForCourse(int userId, int courseId)
        {
            var list = new List<AssignmentSubmission>();
            string query = @"
                SELECT s.*, a.""Title"" AS AssignmentTitle, a.""MilestoneNumber"", a.""TotalMarks""
                FROM ""AssignmentSubmissions"" s
                LEFT JOIN ""MilestoneAssignments"" a ON s.""AssignmentId"" = a.""Id""
                WHERE s.""UserId"" = @userId AND s.""CourseId"" = @courseId
                ORDER BY s.""SubmittedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@courseId", courseId);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new AssignmentSubmission
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                        DriveLink = reader["DriveLink"]?.ToString() ?? "",
                        Note = reader["Note"]?.ToString(),
                        SubmittedAt = reader["SubmittedAt"] as DateTime? ?? DateTime.UtcNow,
                        Marks = reader["Marks"] != DBNull.Value ? Convert.ToInt32(reader["Marks"]) : (int?)null,
                        Feedback = reader["Feedback"]?.ToString(),
                        GradedAt = reader["GradedAt"] as DateTime?,
                        Status = reader["Status"]?.ToString() ?? "Submitted",
                        AssignmentTitle = reader["AssignmentTitle"]?.ToString() ?? "",
                        MilestoneNumber = reader["MilestoneNumber"] != DBNull.Value ? Convert.ToInt32(reader["MilestoneNumber"]) : 0,
                        TotalMarks = reader["TotalMarks"] != DBNull.Value ? Convert.ToInt32(reader["TotalMarks"]) : 0
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetUserSubmissionsForCourse: " + ex.Message);
                return list;
            }
        }

        public (int Total, int Pending, int Graded) GetSubmissionStats()
        {
            string query = @"
                SELECT 
                    COUNT(*) AS total,
                    COUNT(*) FILTER (WHERE ""Status"" != 'Graded') AS pending,
                    COUNT(*) FILTER (WHERE ""Status"" = 'Graded') AS graded
                FROM ""AssignmentSubmissions""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        Convert.ToInt32(reader["total"]),
                        Convert.ToInt32(reader["pending"]),
                        Convert.ToInt32(reader["graded"])
                    );
                }
                return (0, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetSubmissionStats: " + ex.Message);
                return (0, 0, 0);
            }
        }

        // ============================================================
        // ===== ✅ RESUBMIT ELIGIBILITY (50 Points) =====
        // ============================================================

        public (bool CanResubmit, int UserPoints, int RequiredPoints, string Reason)
            CheckResubmitEligibility(int userId, int assignmentId)
        {
            const int COST = AssignmentSubmission.RESUBMIT_COST;

            try
            {
                var submission = GetUserSubmission(userId, assignmentId);
                if (submission == null)
                    return (false, 0, COST, "No submission found.");

                if (!submission.IsGraded)
                    return (false, 0, COST, "Your submission is not yet graded.");

                if (submission.Percentage >= AssignmentSubmission.PASS_PERCENTAGE)
                    return (false, 0, COST, "You already passed this assignment.");

                if (submission.ResubmitCount >= AssignmentSubmission.MAX_RESUBMITS)
                    return (false, 0, COST, $"Maximum resubmit limit ({AssignmentSubmission.MAX_RESUBMITS}) reached.");

                int userPoints = GetUserPoints(userId);
                if (userPoints < COST)
                    return (false, userPoints, COST, $"You have {userPoints} points. Need {COST} points.");

                return (true, userPoints, COST, "Eligible for resubmit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error CheckResubmitEligibility: " + ex.Message);
                return (false, 0, COST, "Error checking eligibility.");
            }
        }

        public bool ResubmitAssignmentWithPoints(int userId, int assignmentId, out string? errorMessage)
        {
            errorMessage = null;

            try
            {
                var (canResubmit, _, _, reason) = CheckResubmitEligibility(userId, assignmentId);
                if (!canResubmit)
                {
                    errorMessage = reason;
                    return false;
                }

                var submission = GetUserSubmission(userId, assignmentId);
                if (submission == null)
                {
                    errorMessage = "Submission not found.";
                    return false;
                }

                if (!SpendPoints(userId, AssignmentSubmission.RESUBMIT_COST,
                    "Spent_Resubmit",
                    $"assignment_{assignmentId}_resubmit",
                    $"Resubmit assignment: {submission.AssignmentTitle}",
                    out string? spendError))
                {
                    errorMessage = spendError;
                    return false;
                }

                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                    UPDATE ""AssignmentSubmissions"" SET
                        ""ResubmitCount"" = ""ResubmitCount"" + 1,
                        ""PointsSpent"" = ""PointsSpent"" + @cost,
                        ""FirstMarks"" = COALESCE(""FirstMarks"", ""Marks""),
                        ""Marks"" = NULL,
                        ""Feedback"" = NULL,
                        ""GradedAt"" = NULL,
                        ""GradedBy"" = NULL,
                        ""Status"" = 'Resubmitted'
                    WHERE ""Id"" = @id", connection);

                command.Parameters.AddWithValue("@id", submission.Id);
                command.Parameters.AddWithValue("@cost", AssignmentSubmission.RESUBMIT_COST);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // ============================================================
        // ===== ✅ RECHECK ELIGIBILITY (50 Points) =====
        // ============================================================

        public (bool CanRecheck, int UserPoints, int RequiredPoints, string Reason)
            CheckRecheckEligibility(int userId, int assignmentId)
        {
            const int COST = AssignmentSubmission.RECHECK_COST;

            try
            {
                var submission = GetUserSubmission(userId, assignmentId);
                if (submission == null)
                    return (false, 0, COST, "No submission found.");

                if (!submission.IsGraded)
                    return (false, 0, COST, "Your submission is not yet graded.");

                if (submission.RecheckCount >= AssignmentSubmission.MAX_RECHECKS)
                    return (false, 0, COST, $"Maximum recheck limit ({AssignmentSubmission.MAX_RECHECKS}) reached.");

                int userPoints = GetUserPoints(userId);
                if (userPoints < COST)
                    return (false, userPoints, COST, $"You have {userPoints} points. Need {COST} points.");

                return (true, userPoints, COST, "Eligible for recheck.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error CheckRecheckEligibility: " + ex.Message);
                return (false, 0, COST, "Error checking eligibility.");
            }
        }

        public bool RequestRecheckWithPoints(int userId, int assignmentId, string? reason,
            out string? errorMessage)
        {
            errorMessage = null;

            try
            {
                var (canRecheck, _, _, eReason) = CheckRecheckEligibility(userId, assignmentId);
                if (!canRecheck)
                {
                    errorMessage = eReason;
                    return false;
                }

                var submission = GetUserSubmission(userId, assignmentId);
                if (submission == null)
                {
                    errorMessage = "Submission not found.";
                    return false;
                }

                if (!SpendPoints(userId, AssignmentSubmission.RECHECK_COST,
                    "Spent_Recheck",
                    $"assignment_{assignmentId}_recheck",
                    $"Recheck request: {submission.AssignmentTitle}",
                    out string? spendError))
                {
                    errorMessage = spendError;
                    return false;
                }

                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                    UPDATE ""AssignmentSubmissions"" SET
                        ""RecheckCount"" = ""RecheckCount"" + 1,
                        ""PointsSpent"" = ""PointsSpent"" + @cost,
                        ""Status"" = 'Recheck_Requested',
                        ""Note"" = COALESCE(""Note"", '') || @recheckNote
                    WHERE ""Id"" = @id", connection);

                command.Parameters.AddWithValue("@id", submission.Id);
                command.Parameters.AddWithValue("@cost", AssignmentSubmission.RECHECK_COST);
                command.Parameters.AddWithValue("@recheckNote",
                    $"\n\n[RECHECK REQUESTED]: {reason ?? "No reason provided"}");

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // ============================================================
        // ===== ✅ LATE SUBMIT CHECK (100 Points) =====
        // ============================================================

        public (bool IsLate, int DaysLate, int PointsCost, string Message)
            CheckLateSubmission(int assignmentId, DateTime submitTime)
        {
            const int LATE_COST = AssignmentSubmission.LATE_SUBMIT_COST;

            try
            {
                var assignment = GetAssignmentById(assignmentId);
                if (assignment == null)
                    return (false, 0, 0, "Assignment not found.");

                if (assignment.CreatedAt == default(DateTime))
                    return (false, 0, 0, "No due date set.");

                var dueDate = assignment.CreatedAt.AddDays(assignment.DueDays);

                if (submitTime <= dueDate)
                    return (false, 0, 0, "Submission on time.");

                var daysLate = (int)Math.Ceiling((submitTime - dueDate).TotalDays);
                return (true, daysLate, LATE_COST,
                    $"This is a LATE submission ({daysLate} day{(daysLate > 1 ? "s" : "")} late). {LATE_COST} points will be deducted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error CheckLateSubmission: " + ex.Message);
                return (false, 0, 0, "Error checking late status.");
            }
        }

        // ============================================================
        // ===== SITE SETTINGS METHODS =====
        // ============================================================

        public SiteSettings GetSiteSettings()
        {
            string query = @"SELECT * FROM ""SiteSettings"" ORDER BY ""Id"" LIMIT 1";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new SiteSettings
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        HeroImageUrls = reader["HeroImageUrls"]?.ToString() ?? "",
                        SliderEnabled = reader["SliderEnabled"] as bool? ?? false,
                        SliderIntervalSeconds = Convert.ToInt32(reader["SliderIntervalSeconds"]),
                        PrimaryColor = reader["PrimaryColor"]?.ToString() ?? "#1F3B2C",
                        SecondaryColor = reader["SecondaryColor"]?.ToString() ?? "#F3F1E7",
                        AccentColor = reader["AccentColor"]?.ToString() ?? "#F4C744",
                        TextColor = reader["TextColor"]?.ToString() ?? "#4B5648",
                        NavbarBgColor = reader["NavbarBgColor"]?.ToString() ?? "#F3F1E7",
                        FooterBgColor = reader["FooterBgColor"]?.ToString() ?? "#F3F1E7",
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UpdatedAt = reader["UpdatedAt"] as DateTime?
                    };
                }
                return new SiteSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting site settings: " + ex.Message);
                return new SiteSettings();
            }
        }

        public bool SaveSiteSettings(SiteSettings settings, out string? errorMessage)
        {
            errorMessage = null;

            string checkQuery = @"SELECT COUNT(*) FROM ""SiteSettings""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                long existing;
                using (var checkCmd = new NpgsqlCommand(checkQuery, connection))
                {
                    existing = Convert.ToInt64(checkCmd.ExecuteScalar());
                }

                string query;
                if (existing > 0)
                {
                    query = @"
                        UPDATE ""SiteSettings"" SET
                            ""HeroImageUrls"" = @heroImageUrls,
                            ""SliderEnabled"" = @sliderEnabled,
                            ""SliderIntervalSeconds"" = @sliderInterval,
                            ""PrimaryColor"" = @primaryColor,
                            ""SecondaryColor"" = @secondaryColor,
                            ""AccentColor"" = @accentColor,
                            ""TextColor"" = @textColor,
                            ""NavbarBgColor"" = @navbarBgColor,
                            ""FooterBgColor"" = @footerBgColor,
                            ""UpdatedAt"" = @updatedAt
                        WHERE ""Id"" = @id";
                }
                else
                {
                    query = @"
                        INSERT INTO ""SiteSettings"" 
                        (""HeroImageUrls"", ""SliderEnabled"", ""SliderIntervalSeconds"",
                         ""PrimaryColor"", ""SecondaryColor"", ""AccentColor"", ""TextColor"",
                         ""NavbarBgColor"", ""FooterBgColor"", ""CreatedAt"")
                        VALUES 
                        (@heroImageUrls, @sliderEnabled, @sliderInterval,
                         @primaryColor, @secondaryColor, @accentColor, @textColor,
                         @navbarBgColor, @footerBgColor, @createdAt)";
                }

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@heroImageUrls", settings.HeroImageUrls ?? "");
                command.Parameters.AddWithValue("@sliderEnabled", settings.SliderEnabled);
                command.Parameters.AddWithValue("@sliderInterval", settings.SliderIntervalSeconds);
                command.Parameters.AddWithValue("@primaryColor", settings.PrimaryColor ?? "#1F3B2C");
                command.Parameters.AddWithValue("@secondaryColor", settings.SecondaryColor ?? "#F3F1E7");
                command.Parameters.AddWithValue("@accentColor", settings.AccentColor ?? "#F4C744");
                command.Parameters.AddWithValue("@textColor", settings.TextColor ?? "#4B5648");
                command.Parameters.AddWithValue("@navbarBgColor", settings.NavbarBgColor ?? "#F3F1E7");
                command.Parameters.AddWithValue("@footerBgColor", settings.FooterBgColor ?? "#F3F1E7");

                if (existing > 0)
                {
                    command.Parameters.AddWithValue("@id", settings.Id);
                    command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                }
                else
                {
                    command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                }

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // ============================================================
        // ===== PRIVATE MAPPERS =====
        // ============================================================

        private User MapUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserName = reader["UserName"]?.ToString() ?? string.Empty,
                Password = reader["Password"]?.ToString() ?? string.Empty,
                FullName = reader["FullName"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                LastLoginAt = reader["LastLoginAt"] as DateTime?,
                Role = reader["Role"]?.ToString() ?? "User",
                IsActive = reader["IsActive"] as bool? ?? true,
                TotalPoints = reader["TotalPoints"] != DBNull.Value ? Convert.ToInt32(reader["TotalPoints"]) : 0,
                MobileNumber = reader["MobileNumber"]?.ToString(),
                ProfileImage = reader["ProfileImage"]?.ToString(),
                Gender = reader["Gender"]?.ToString(),
                AgeRange = reader["AgeRange"]?.ToString(),
                PrimaryDeviceType = reader["PrimaryDeviceType"]?.ToString(),
                YearsOfExperience = reader["YearsOfExperience"]?.ToString(),
                AreaType = reader["AreaType"]?.ToString(),
                Country = reader["Country"]?.ToString(),
                StreetAddress = reader["StreetAddress"]?.ToString(),
                PermanentAddress = reader["PermanentAddress"]?.ToString(),
                EducationLevel = reader["EducationLevel"]?.ToString(),
                CurrentStudyStatus = reader["CurrentStudyStatus"]?.ToString(),
                ExamDegreeTitle = reader["ExamDegreeTitle"]?.ToString(),
                InstitutionName = reader["InstitutionName"]?.ToString(),
                PassingYear = reader["PassingYear"]?.ToString(),
                IsCSEStudent = reader["IsCSEStudent"] as bool?,
                CvLink = reader["CvLink"]?.ToString(),
                GithubProfile = reader["GithubProfile"]?.ToString(),
                PortfolioLink = reader["PortfolioLink"]?.ToString(),
                LinkedInProfile = reader["LinkedInProfile"]?.ToString(),
                ProfileImageLink = reader["ProfileImageLink"]?.ToString()
            };
        }

        private TeacherRequest MapTeacherRequest(NpgsqlDataReader reader)
        {
            return new TeacherRequest
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                FullName = reader["FullName"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                MobileNumber = reader["MobileNumber"]?.ToString() ?? string.Empty,
                Education = reader["Education"]?.ToString() ?? string.Empty,
                Institution = reader["Institution"]?.ToString() ?? string.Empty,
                SubjectExpertise = reader["SubjectExpertise"]?.ToString() ?? string.Empty,
                Experience = reader["Experience"]?.ToString() ?? string.Empty,
                TeachingStyle = reader["TeachingStyle"]?.ToString() ?? string.Empty,
                AvailableDays = reader["AvailableDays"]?.ToString() ?? string.Empty,
                PreferredTime = reader["PreferredTime"]?.ToString() ?? string.Empty,
                HourlyRate = reader["HourlyRate"]?.ToString() ?? string.Empty,
                CvLink = reader["CvLink"]?.ToString() ?? string.Empty,
                WhyTeach = reader["WhyTeach"]?.ToString() ?? string.Empty,
                Status = reader["Status"]?.ToString() ?? "Pending",
                RequestDate = reader["RequestDate"] as DateTime? ?? DateTime.UtcNow,
                ResponseDate = reader["ResponseDate"] as DateTime?,
                AdminNote = reader["AdminNote"]?.ToString()
            };
        }

        private CourseOrder MapCourseOrder(NpgsqlDataReader reader)
        {
            return new CourseOrder
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                CourseName = reader["CourseName"]?.ToString() ?? "",
                CourseImage = reader["CourseImage"]?.ToString() ?? "",
                Price = Convert.ToDecimal(reader["Price"]),
                PaymentMethod = reader["PaymentMethod"]?.ToString() ?? "",
                TransactionId = reader["TransactionId"]?.ToString() ?? "",
                SenderMobileNumber = reader["SenderMobileNumber"]?.ToString() ?? "",
                PaymentStatus = reader["PaymentStatus"]?.ToString() ?? "Pending",
                AdminNote = reader["AdminNote"]?.ToString(),
                OrderDate = reader["OrderDate"] as DateTime? ?? DateTime.UtcNow,
                VerifiedDate = reader["VerifiedDate"] as DateTime?,
                VerifiedBy = reader["VerifiedBy"] as int?
            };
        }

        private Course MapCourse(NpgsqlDataReader reader)
        {
            return new Course
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader["Title"]?.ToString() ?? "",
                Description = reader["Description"]?.ToString() ?? "",
                Image = reader["Image"]?.ToString() ?? "",
                Category = reader["Category"]?.ToString() ?? "",
                Price = Convert.ToDecimal(reader["Price"]),
                Students = Convert.ToInt32(reader["Students"]),
                Rating = Convert.ToDouble(reader["Rating"]),
                Instructor = reader["Instructor"]?.ToString() ?? "",
                Duration = reader["Duration"]?.ToString() ?? "",
                Lessons = Convert.ToInt32(reader["Lessons"]),
                Level = reader["Level"]?.ToString() ?? "",
                IsNew = reader["IsNew"] as bool? ?? true,
                IsPopular = reader["IsPopular"] as bool? ?? false,
                IsEnrollmentOpen = reader["IsEnrollmentOpen"] as bool? ?? true,
                Features = reader["Features"]?.ToString(),
                CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow
            };
        }

        private Notice MapNotice(NpgsqlDataReader reader)
        {
            return new Notice
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader["Title"]?.ToString() ?? "",
                Content = reader["Content"]?.ToString() ?? "",
                Category = reader["Category"]?.ToString() ?? "News",
                ImageUrl = reader["ImageUrl"]?.ToString() ?? "",
                PublishedDate = reader["PublishedDate"] as DateTime? ?? DateTime.UtcNow,
                IsAnnouncement = reader["IsAnnouncement"] as bool? ?? false,
                IsActive = reader["IsActive"] as bool? ?? true,
                CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                UpdatedAt = reader["UpdatedAt"] as DateTime?
            };
        }

        private MilestoneAssignment MapAssignment(NpgsqlDataReader reader)
        {
            return new MilestoneAssignment
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                MilestoneNumber = Convert.ToInt32(reader["MilestoneNumber"]),
                Title = reader["Title"]?.ToString() ?? "",
                Description = reader["Description"]?.ToString(),
                Instructions = reader["Instructions"]?.ToString(),
                TotalMarks = Convert.ToInt32(reader["TotalMarks"]),
                DueDays = Convert.ToInt32(reader["DueDays"]),
                IsPublished = reader["IsPublished"] as bool? ?? false,
                CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                UpdatedAt = reader["UpdatedAt"] as DateTime?
            };
        }
    }
}