// 📁 DatabaseHelper.cs
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
            // ===== শুধু trim (lowercase করব না) =====
            var trimmedEmail = (user.Email ?? string.Empty).Trim();

            // ===== @gmail.com হুবহু lowercase-এ শেষ হয়েছে কিনা চেক =====
            if (string.IsNullOrWhiteSpace(trimmedEmail) ||
                !trimmedEmail.EndsWith("@gmail.com"))
            {
                throw new Exception("Email must end with @gmail.com (all lowercase).");
            }

            // ===== Gmail ফরম্যাট ভ্যালিডেশন =====
            var gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@gmail\.com$");
            if (!gmailRegex.IsMatch(trimmedEmail))
            {
                throw new Exception("Invalid Gmail address format.");
            }

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
            // Duplicate ধরার জন্য case-insensitive চেক
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
    }
}