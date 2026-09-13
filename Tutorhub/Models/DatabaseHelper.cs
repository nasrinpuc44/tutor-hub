// 📁 DatabaseHelper.cs
// লোকেশন: Tutorbub/DatabaseHelper.cs

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
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

        // ===== সব ইউজার আনা =====
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
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
                    users.Add(new User
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
                    });
                }
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all users: " + ex.Message);
            }
        }

        // ===== ইউজার অথেন্টিকেট করা =====
        public User? AuthenticateUser(string username, string password)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                WHERE ""UserName"" = @username AND ""Password"" = @password AND ""IsActive"" = TRUE";

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
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user: " + ex.Message);
            }
        }

        // ===== ইউজার প্রোফাইল আপডেট করা =====
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

        // ===== ইউজার আইডি দিয়ে খোঁজা =====
        public User? GetUserById(int userId)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
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
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user by ID: " + ex.Message);
            }
        }

        // ===== নতুন ইউজার রেজিস্টার =====
        public bool RegisterUser(User user)
        {
            string query = @"
                INSERT INTO ""Users"" (""UserName"", ""Password"", ""FullName"", ""Email"", ""Role"")
                VALUES (@username, @password, @fullname, @email, 'User')";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", user.UserName);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@fullname", user.FullName);
                command.Parameters.AddWithValue("@email", user.Email);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user: " + ex.Message);
            }
        }

        // ===== ইউজার ডিলিট (সম্পর্কিত Teacher Request সহ) =====
        // ইউজার ডিলিট করার আগে তার সব TeacherRequests রো ডিলিট করা হয়, যাতে
        // ফরেন কী কনস্ট্রেইন্টের কারণে ডিলিট ব্যর্থ না হয় (টিচার ডিলিট করলে
        // তার রিকোয়েস্ট হিস্ট্রিও ডাটাবেজ থেকে মুছে যায়)।
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

        // ===== পাসওয়ার্ড আপডেট =====
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

        // ===== ইউজার স্ট্যাটাস টগল =====
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

        // ===== ইউজারনেম চেক =====
        public bool UsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM \"Users\" WHERE \"UserName\" = @username";
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

        // ===== ইমেইল চেক =====
        public bool EmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM \"Users\" WHERE \"Email\" = @email";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking email: " + ex.Message);
            }
        }

        // ===== লাস্ট লগইন আপডেট =====
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
        // ===== TEACHER REQUEST RELATED METHODS =====
        // ============================================================

        // ===== Teacher Request তৈরি করা =====
        // errorMessage আউট প্যারামিটারে আসল exception message পাঠানো হয়, যাতে UI-তে
        // দেখানো যায় ঠিক কী কারণে insert ব্যর্থ হয়েছে (যেমন: টেবিল নেই, কলাম টাইপ মিসম্যাচ ইত্যাদি)।
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
                Console.WriteLine($"CreateTeacherRequest: {result} rows affected, UserId: {request.UserId}");
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating teacher request: " + ex.Message);
                Console.WriteLine("Stack trace: " + ex.StackTrace);
                errorMessage = ex.Message;
                return false;
            }
        }

        // ===== ইউজারের টিচার রিকোয়েস্ট স্ট্যাটাস চেক করা =====
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
            catch (Exception ex)
            {
                Console.WriteLine("Error getting teacher request status: " + ex.Message);
                return "None";
            }
        }

        // ===== সব Teacher Request পাওয়া =====
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
                    requests.Add(new TeacherRequest
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
                    });
                }
                Console.WriteLine($"GetAllTeacherRequests: {requests.Count} requests found");
                return requests;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting teacher requests: " + ex.Message);
                throw new Exception("Error getting teacher requests: " + ex.Message);
            }
        }

        // ===== Teacher Request আপডেট করা =====
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

        // ===== Teacher Request দ্বারা ইউজার পাওয়া =====
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
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting teacher request: " + ex.Message);
            }
        }

        // ===== ইউজারকে টিচার বানানো =====
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

        // ===== Teacher Request নিজের Id দিয়ে সরাসরি ডিলিট =====
        // যদি এই request-এর সাথে জড়িত UserId এর কোনো ইউজার Users টেবিলে
        // না থাকে (যেমন পুরনো/টেস্ট ডেটা), তাহলেও এই মেথড দিয়ে শুধু এই
        // নির্দিষ্ট request রো-টা মুছে ফেলা যাবে।
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
    }
}