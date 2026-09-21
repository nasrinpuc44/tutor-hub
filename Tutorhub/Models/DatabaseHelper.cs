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

        // ===== কোর্স ডিলিট =====
        public bool DeleteCourse(int courseId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using (var deleteLessons = new NpgsqlCommand(
                    @"DELETE FROM ""CourseLessons"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteLessons.Parameters.AddWithValue("@id", courseId);
                    deleteLessons.ExecuteNonQuery();
                }

                using (var deleteOrders = new NpgsqlCommand(
                    @"DELETE FROM ""CourseOrders"" WHERE ""CourseId"" = @id", connection))
                {
                    deleteOrders.Parameters.AddWithValue("@id", courseId);
                    deleteOrders.ExecuteNonQuery();
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

        // ===== Enrollment Open/Close Toggle =====
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
        // ===== COURSE LESSON / VIDEO METHODS (নতুন) =====
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
    }
}