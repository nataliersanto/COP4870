using Library.PoS.Model;
using Library.PoS.Services;

namespace CLI.PoS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine("\n=== Learning Management System ===");
                Console.WriteLine("1. Teacher");
                Console.WriteLine("2. Student");
                Console.WriteLine("3. Quit");

                choice = Console.ReadLine();
                if (int.TryParse(choice, out int choiceInt))
                {
                    switch (choiceInt)
                    {
                        case 1:
                            TeacherMenu();
                            break;
                        case 2:
                            StudentMenu();
                            break;
                        case 3:
                            Console.WriteLine("Goodbye!");
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
            } while (!choice.Equals("3", StringComparison.OrdinalIgnoreCase));
        }

        static void TeacherMenu()
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine("\n=== Teacher Menu ===");
                Console.WriteLine("1. Manage Courses");
                Console.WriteLine("2. Manage Students");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "1":
                        ManageCoursesMenu();
                        break;
                    case "2":
                        ManageStudentsMenu();
                        break;
                    case "Q":
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void ManageCoursesMenu()
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine("\n=== Manage Courses ===");
                Console.WriteLine("C. Create Course");
                Console.WriteLine("R. List Courses");
                Console.WriteLine("U. Edit Course");
                Console.WriteLine("D. Delete Course");
                Console.WriteLine("S. Select Course");
                Console.WriteLine("X. Copy Course");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "C":
                        CreateCourse();
                        break;
                    case "R":
                        ListCourses();
                        break;
                    case "U":
                        EditCourse();
                        break;
                    case "D":
                        DeleteCourse();
                        break;
                    case "S":
                        SelectCourse();
                        break;
                    case "X":
                        CopyCourse();
                        break;
                    case "Q":
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void CreateCourse()
        {
            Console.WriteLine("\n=== Create Course ===");
            Console.Write("Course Name: ");
            var name = Console.ReadLine();
            Console.Write("Course Code: ");
            var code = Console.ReadLine();
            Console.Write("Description: ");
            var description = Console.ReadLine();
            Console.Write("Semester (e.g. Fall 2024): ");
            var semester = Console.ReadLine();
            Console.Write("Section: ");
            var section = Console.ReadLine();

            var course = new Course
            {
                Name = name,
                Code = code,
                Description = description,
                Semester = semester,
                Section = section
            };
            CourseServiceProxy.Current.AddOrUpdate(course);
            Console.WriteLine($"Course created: {course}");
        }

        static void ListCourses()
        {
            Console.WriteLine("\n=== Courses by Semester ===");
            var courses = CourseServiceProxy.Current.Courses;
            if (!courses.Any())
            {
                Console.WriteLine("No courses found.");
                return;
            }
            var bySemester = courses.GroupBy(c => c.Semester ?? "Unknown");
            foreach (var group in bySemester)
            {
                Console.WriteLine($"\n--- {group.Key} ---");
                group.ToList().ForEach(Console.WriteLine);
            }
        }

        static void EditCourse()
        {
            ListCourses();
            Console.Write("Enter Course ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var course = CourseServiceProxy.Current.GetById(id);
            if (course == null) { Console.WriteLine("Course not found."); return; }

            Console.Write($"New Name ({course.Name}): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)) course.Name = name;

            Console.Write($"New Description ({course.Description}): ");
            var desc = Console.ReadLine();
            if (!string.IsNullOrEmpty(desc)) course.Description = desc;

            CourseServiceProxy.Current.AddOrUpdate(course);
            Console.WriteLine("Course updated!");
        }

        static void DeleteCourse()
        {
            ListCourses();
            Console.Write("Enter Course ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var deleted = CourseServiceProxy.Current.Delete(id);
            Console.WriteLine(deleted != null ? $"Deleted: {deleted.Name}" : "Course not found.");
        }
        
        static void CopyCourse()
        {
            ListCourses();
            Console.Write("Enter Course ID to copy: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var original = CourseServiceProxy.Current.GetById(id);
            if (original == null) { Console.WriteLine("Course not found."); return; }

            var copy = new Course
            {
                Name = original.Name + " (Copy)",
                Code = original.Code,
                Description = original.Description,
                Semester = original.Semester,
                Section = original.Section,
                Modules = original.Modules.Select(m => new Module
                {
                    Id = m.Id,
                    Name = m.Name,
                    Content = new List<string>(m.Content)
                }).ToList(),
                Assignments = original.Assignments.Select(a => new Assignment
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    AvailablePoints = a.AvailablePoints,
                    DueDate = a.DueDate,
                    Submissions = new List<Submission>() // don't copy submissions
                }).ToList()
            };
            CourseServiceProxy.Current.AddOrUpdate(copy);
            Console.WriteLine($"Course copied: {copy}");
        }
        
        static void ManageAssignmentGroups(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== Assignment Groups: {course.Name} ===");
                Console.WriteLine("C. Create Group");
                Console.WriteLine("R. List Groups");
                Console.WriteLine("U. Edit Group");
                Console.WriteLine("D. Delete Group");
                Console.WriteLine("A. Add Assignment to Group");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "C":
                        Console.Write("Group Name: ");
                        var name = Console.ReadLine();
                        Console.Write("Weight (e.g. 0.4 for 40%): ");
                        double.TryParse(Console.ReadLine(), out double weight);
                        var group = new AssignmentGroup
                        {
                            Id = course.AssignmentGroups.Any() ? course.AssignmentGroups.Max(g => g.Id) + 1 : 1,
                            Name = name,
                            Weight = weight
                        };
                    course.AssignmentGroups.Add(group);
                    Console.WriteLine($"Group created: {group}");
                    break;
                    case "R":
                        course.AssignmentGroups.ForEach(Console.WriteLine);
                        break;
                    case "U":
                        course.AssignmentGroups.ForEach(Console.WriteLine);
                        Console.Write("Enter Group ID to edit: ");
                        if (int.TryParse(Console.ReadLine(), out int editId))
                        {
                            var g = course.AssignmentGroups.FirstOrDefault(g => g.Id == editId);
                            if (g != null)
                            {
                                Console.Write($"New Name ({g.Name}): ");
                                var newName = Console.ReadLine();
                                if (!string.IsNullOrEmpty(newName)) g.Name = newName;
                               Console.Write($"New Weight ({g.Weight:P0}): ");
                               if (double.TryParse(Console.ReadLine(), out double newWeight)) g.Weight = newWeight;
                               Console.WriteLine("Group updated!");
                            }
                        }
                        break;
                    case "D":
                        course.AssignmentGroups.ForEach(Console.WriteLine);
                        Console.Write("Enter Group ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int delId))
                        {
                            var g = course.AssignmentGroups.FirstOrDefault(g => g.Id == delId);
                            if (g != null) { course.AssignmentGroups.Remove(g); Console.WriteLine("Deleted."); }
                        }
                        break;
                    case "A":
                        course.AssignmentGroups.ForEach(Console.WriteLine);
                        Console.Write("Enter Group ID: ");
                        if (int.TryParse(Console.ReadLine(), out int groupId))
                        {
                            var g = course.AssignmentGroups.FirstOrDefault(g => g.Id == groupId);
                            if (g != null)
                            {
                                course.Assignments.ForEach(Console.WriteLine);
                                Console.Write("Enter Assignment ID to add: ");
                                if (int.TryParse(Console.ReadLine(), out int aId))
                                {
                                    var a = course.Assignments.FirstOrDefault(a => a.Id == aId);
                                    if (a != null) { g.Assignments.Add(a); Console.WriteLine($"Added {a.Name} to {g.Name}"); }
                                }
                            }
                        }
                        break;
                    case "Q":
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }
        
        static void SelectCourse()
        {
            ListCourses();
            Console.Write("Enter Course ID to manage: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var course = CourseServiceProxy.Current.GetById(id);
            if (course == null) { Console.WriteLine("Course not found."); return; }
            CourseDetailMenu(course);
        }

        static void CourseDetailMenu(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== {course.Name} ({course.Code}) ===");
                Console.WriteLine("1. Manage Roster");
                Console.WriteLine("2. Manage Assignments");
                Console.WriteLine("3. Manage Modules");
                Console.WriteLine("4. Grade Submissions");
                Console.WriteLine("5. Manage Assignment Groups");
                Console.WriteLine("6. Manage Grade Ranges");
                Console.WriteLine("7. Export Gradebook (CSV)");
                Console.WriteLine("8. Export Assignments");
                Console.WriteLine("9. Import Assignments");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "1":
                        ManageRoster(course);
                        break;
                    case "2":
                        ManageAssignments(course);
                        break;
                    case "3":
                        ManageModules(course);
                        break;
                    case "4":
                        GradeSubmissions(course);
                        break;
                    case "5":
                        ManageAssignmentGroups(course);
                        break;
                    case "6":
                        ManageGradeRanges(course);
                        break;
                    case "7":
                        ExportGradebook(course);
                        break;
                    case "8":
                        ExportAssignments(course);
                        break;
                    case "9":
                        ImportAssignments(course);
                        break;
                    case "Q":
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void ManageRoster(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== Roster: {course.Name} ===");
                Console.WriteLine("A. Add Student");
                Console.WriteLine("R. List Students");
                Console.WriteLine("D. Remove Student");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "A":
                        EnrollStudent(course);
                        break;
                    case "R":
                        course.Roster.ForEach(Console.WriteLine);
                        break;
                    case "D":
                        UnenrollStudent(course);
                        break;
                    case "Q":
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void EnrollStudent(Course course)
        {
            Console.WriteLine("\nAll Students:");
            StudentServiceProxy.Current.Students.ForEach(Console.WriteLine);
            Console.Write("Enter Student ID to enroll (or 0 to create new): ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            Student? student;
            if (id == 0)
            {
                Console.Write("Name: ");
                var name = Console.ReadLine();
                Console.Write("FSUID: ");
                var code = Console.ReadLine();
                Console.Write("Classification: ");
                var classification = Console.ReadLine();
                student = new Student { Name = name, Code = code, Classification = classification };
                StudentServiceProxy.Current.AddOrUpdate(student);
            }
            else
            {
                student = StudentServiceProxy.Current.GetById(id);
            }

            if (student == null) { Console.WriteLine("Student not found."); return; }
            if (course.Roster.Any(s => s.Id == student.Id))
            {
                Console.WriteLine("Student already enrolled.");
                return;
            }
            course.Roster.Add(student);
            Console.WriteLine($"Enrolled {student.Name} in {course.Name}");
        }

        static void UnenrollStudent(Course course)
        {
            course.Roster.ForEach(Console.WriteLine);
            Console.Write("Enter Student ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var student = course.Roster.FirstOrDefault(s => s.Id == id);
            if (student == null) { Console.WriteLine("Student not found in roster."); return; }
            course.Roster.Remove(student);
            Console.WriteLine($"Removed {student.Name} from {course.Name}");
        }

        static void ManageAssignments(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== Assignments: {course.Name} ===");
                Console.WriteLine("C. Create Assignment");
                Console.WriteLine("R. List Assignments");
                Console.WriteLine("U. Edit Assignment");
                Console.WriteLine("D. Delete Assignment");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "C":
                        CreateAssignment(course);
                        break;
                    case "R":
                        course.Assignments.ForEach(Console.WriteLine);
                        break;
                    case "U":
                        EditAssignment(course);
                        break;
                    case "D":
                        DeleteAssignment(course);
                        break;
                    case "Q":
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void CreateAssignment(Course course)
        {
            Console.Write("Is this a Quiz? (Y/N): ");
            var isQuiz = Console.ReadLine()?.ToUpper() == "Y";

            Console.Write("Assignment Name: ");
            var name = Console.ReadLine();
            Console.Write("Description: ");
            var desc = Console.ReadLine();
            Console.Write("Available Points: ");
            int.TryParse(Console.ReadLine(), out int points);
            Console.Write("Due Date (MM/DD/YYYY): ");
            DateTime.TryParse(Console.ReadLine(), out DateTime due);

            var assignment = new Assignment
            {
                Id = course.Assignments.Any() ? course.Assignments.Max(a => a.Id) + 1 : 1,
                Name = name,
                Description = desc,
                AvailablePoints = points,
                DueDate = due,
                IsQuiz = isQuiz
            };

            if (isQuiz)
            {
                var qChoice = string.Empty;
                do
                {
                    //quiz is of type List<QuizQuestion>
                    Console.WriteLine("\nQuiz Questions:");
                    assignment.Questions.Select((q, i) => $"{i + 1}. {q.Prompt}").ToList().ForEach(Console.WriteLine);
                    Console.WriteLine("A. Add Free Text Question");
                    Console.WriteLine("M. Add Multiple Choice Question");
                    Console.WriteLine("Q. Done");
                    qChoice = Console.ReadLine();

                    if (qChoice?.ToUpper() == "A")
                    {
                        Console.Write("Question: ");
                        assignment.Questions.Add(new QuizQuestion { Prompt = Console.ReadLine() });
                    }
                    else if (qChoice?.ToUpper() == "M")
                    {
                        Console.Write("Question: ");
                        var prompt = Console.ReadLine();
                        var question = new QuizQuestion { Prompt = prompt };

                        var addMore = true;
                        while (addMore)
                        {
                            Console.Write("Add choice (or leave blank to stop): ");
                            var choiceText = Console.ReadLine();
                            if (string.IsNullOrEmpty(choiceText))
                                addMore = false;
                            else
                                question.Choices.Add(choiceText);
                        }
                        assignment.Questions.Add(question);
                    }
                } while (!qChoice.Equals("Q", StringComparison.OrdinalIgnoreCase));
            }

            course.Assignments.Add(assignment);
            Console.WriteLine($"Created: {assignment}");
        }

        static void EditAssignment(Course course)
        {
            course.Assignments.ForEach(Console.WriteLine);
            Console.Write("Enter Assignment ID to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == id);
            if (assignment == null) { Console.WriteLine("Not found."); return; }

            Console.Write($"New Name ({assignment.Name}): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrEmpty(name)) assignment.Name = name;

            Console.Write($"New Description ({assignment.Description}): ");
            var desc = Console.ReadLine();
            if (!string.IsNullOrEmpty(desc)) assignment.Description = desc;

            Console.WriteLine("Assignment updated!");
        }

        static void DeleteAssignment(Course course)
        {
            course.Assignments.ForEach(Console.WriteLine);
            Console.Write("Enter Assignment ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == id);
            if (assignment == null) { Console.WriteLine("Not found."); return; }
            course.Assignments.Remove(assignment);
            Console.WriteLine("Assignment deleted.");
        }

        static void ManageModules(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== Modules: {course.Name} ===");
                Console.WriteLine("C. Create Module");
                Console.WriteLine("R. List Modules");
                Console.WriteLine("D. Delete Module");
                Console.WriteLine("S. Select Module to manage content");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "C":
                        var m = new Module
                        {
                            Id = course.Modules.Any() ? course.Modules.Max(m => m.Id) + 1 : 1
                        };
                        Console.Write("Module Name: ");
                        m.Name = Console.ReadLine();
                        course.Modules.Add(m);
                        Console.WriteLine($"Module created: {m}");
                        break;
                    case "R":
                        course.Modules.ForEach(Console.WriteLine);
                        break;
                    case "D":
                        course.Modules.ForEach(Console.WriteLine);
                        Console.Write("Enter Module ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int delId))
                        {
                            var mod = course.Modules.FirstOrDefault(m => m.Id == delId);
                            if (mod != null) { course.Modules.Remove(mod); Console.WriteLine("Deleted."); }
                        }
                        break;
                    case "S":
                        course.Modules.ForEach(Console.WriteLine);
                        Console.Write("Enter Module ID: ");
                        if (int.TryParse(Console.ReadLine(), out int selId))
                        {
                            var mod = course.Modules.FirstOrDefault(m => m.Id == selId);
                            if (mod != null) ManageModuleContent(mod);
                        }
                        break;
                    case "Q":
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void ManageModuleContent(Module module)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== Module: {module.Name} ===");
                module.Content.Select((c, i) => $"{i + 1}. {c}").ToList().ForEach(Console.WriteLine);
                Console.WriteLine("A. Add Content");
                Console.WriteLine("U. Update Content");
                Console.WriteLine("D. Remove Content");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "A":
                        Console.Write("Content: ");
                        module.Content.Add(Console.ReadLine() ?? "");
                        break;
                    case "U":
                        Console.Write("Content number to update: ");
                        if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= module.Content.Count)
                        {
                            Console.Write("New content: ");
                            module.Content[idx - 1] = Console.ReadLine() ?? "";
                        }
                        break;
                    case "D":
                        Console.Write("Content number to remove: ");
                        if (int.TryParse(Console.ReadLine(), out int dIdx) && dIdx > 0 && dIdx <= module.Content.Count)
                            module.Content.RemoveAt(dIdx - 1);
                        break;
                    case "Q":
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void GradeSubmissions(Course course)
        {
            Console.WriteLine($"\n=== Grade Submissions: {course.Name} ===");
            var allSubmissions = course.Assignments.SelectMany(a => a.Submissions).ToList();
            if (!allSubmissions.Any()) { Console.WriteLine("No submissions yet."); return; }

            allSubmissions.ForEach(s =>
            {
                var student = StudentServiceProxy.Current.GetById(s.StudentId);
                var assignment = course.Assignments.FirstOrDefault(a => a.Id == s.AssignmentId);
                Console.WriteLine($"\nStudent: {student?.Name} | Assignment: {assignment?.Name}");
                Console.WriteLine($"Content: {s.Content}");
                Console.Write($"Enter grade (out of {assignment?.AvailablePoints}): ");
                if (int.TryParse(Console.ReadLine(), out int grade)) s.Grade = grade;
                Console.Write("Feedback: ");
                s.Feedback = Console.ReadLine();
            });
        }
        
        static void ManageGradeRanges(Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== Grade Ranges: {course.Name} ===");
                foreach (var range in course.GradeRanges.OrderByDescending(r => r.Value))
                    Console.WriteLine($"{range.Key}: {range.Value}%+");

                Console.WriteLine("U. Update a grade range");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                if (choice?.ToUpper() == "U")
                {
                    Console.Write("Letter grade to update (A/B/C/D/F): ");
                    var letter = Console.ReadLine()?.ToUpper();
                    if (letter != null && course.GradeRanges.ContainsKey(letter))
                    {
                        Console.Write($"New minimum percentage for {letter}: ");
                        if (double.TryParse(Console.ReadLine(), out double newMin))
                        {
                            course.GradeRanges[letter] = newMin;
                            Console.WriteLine("Updated!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid letter grade.");
                    }
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }
        
        static void ExportGradebook(Course course)
        {
            Console.WriteLine($"\n=== Export Gradebook: {course.Name} ===");

            var sb = new System.Text.StringBuilder();

            //header
            sb.Append("Student Name,FSUID");
            foreach (var assignment in course.Assignments)
                sb.Append($",{assignment.Name} ({assignment.AvailablePoints} pts)");
            sb.AppendLine(",Average,Letter Grade");

            //student
            foreach (var student in course.Roster)
            {
                sb.Append($"{student.Name},{student.Code}");
                var totalScore = 0.0;
                var totalPoints = 0;

                foreach (var assignment in course.Assignments)
                {
                    var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
                    var grade = submission?.Grade?.ToString() ?? "N/A";
                    sb.Append($",{grade}");
                    if (submission?.Grade != null)
                    {
                        totalScore += submission.Grade.Value;
                        totalPoints += assignment.AvailablePoints;
                    }
                }

                if (totalPoints > 0)
                {
                    double avg = totalScore / totalPoints * 100;
                    string letter = course.GetLetterGrade(avg);
                    sb.AppendLine($",{avg:F1}%,{letter}");
                }
                else
                {
                    sb.AppendLine(",N/A,N/A");
                }
            }

            //save to file
            var fileName = $"{course.Code}_gradebook.csv";
            File.WriteAllText(fileName, sb.ToString());
            Console.WriteLine($"Gradebook exported to {fileName}");
        }
        
        static void ExportAssignments(Course course)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Id,Name,Description,AvailablePoints,DueDate,IsQuiz");
            foreach (var a in course.Assignments)
                sb.AppendLine($"{a.Id},{a.Name},{a.Description},{a.AvailablePoints},{a.DueDate:MM/dd/yyyy},{a.IsQuiz}");

            var fileName = $"{course.Code}_assignments.csv";
            File.WriteAllText(fileName, sb.ToString());
            Console.WriteLine($"Assignments exported to {fileName}");
        }
        
        static void ImportAssignments(Course course)
        {
            Console.Write("Enter path to assignments CSV file: ");
            var filePath = Console.ReadLine();
            if (!File.Exists(filePath)) { Console.WriteLine("File not found."); return; }

            var lines = File.ReadAllLines(filePath).Skip(1); // skip header
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 6) continue;
                var assignment = new Assignment
                {
                    Id = course.Assignments.Any() ? course.Assignments.Max(a => a.Id) + 1 : 1,
                    Name = parts[1],
                    Description = parts[2],
                    AvailablePoints = int.TryParse(parts[3], out int pts) ? pts : 0,
                    DueDate = DateTime.TryParse(parts[4], out DateTime due) ? due : DateTime.Now,
                    IsQuiz = parts[5].Trim().ToLower() == "true"
                };
                course.Assignments.Add(assignment);
            }
            Console.WriteLine("Assignments imported!");
        }

        static void ManageStudentsMenu()
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine("\n=== Manage Students ===");
                Console.WriteLine("C. Add Student");
                Console.WriteLine("R. List Students");
                Console.WriteLine("U. Edit Student");
                Console.WriteLine("D. Delete Student");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "C":
                        Console.Write("Name: ");
                        var name = Console.ReadLine();
                        Console.Write("FSUID: ");
                        var code = Console.ReadLine();
                        Console.Write("Classification: ");
                        var classification = Console.ReadLine();
                        var student = new Student { Name = name, Code = code, Classification = classification };
                        StudentServiceProxy.Current.AddOrUpdate(student);
                        Console.WriteLine($"Student added: {student}");
                        break;
                    case "R":
                        StudentServiceProxy.Current.Students.ForEach(Console.WriteLine);
                        break;
                    case "U":
                        StudentServiceProxy.Current.Students.ForEach(Console.WriteLine);
                        Console.Write("Enter Student ID to edit: ");
                        if (int.TryParse(Console.ReadLine(), out int editId))
                        {
                            var s = StudentServiceProxy.Current.GetById(editId);
                            if (s != null)
                            {
                                Console.Write($"New Name ({s.Name}): ");
                                var newName = Console.ReadLine();
                                if (!string.IsNullOrEmpty(newName)) s.Name = newName;
                                Console.Write($"New Classification ({s.Classification}): ");
                                var newClass = Console.ReadLine();
                                if (!string.IsNullOrEmpty(newClass)) s.Classification = newClass;
                                StudentServiceProxy.Current.AddOrUpdate(s);
                                Console.WriteLine("Student updated!");
                            }
                        }
                        break;
                    case "D":
                        StudentServiceProxy.Current.Students.ForEach(Console.WriteLine);
                        Console.Write("Enter Student ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int delId))
                        {
                            var deleted = StudentServiceProxy.Current.Delete(delId);
                            // Also remove from all course rosters
                            CourseServiceProxy.Current.Courses.ForEach(c =>
                                c.Roster.RemoveAll(s => s.Id == delId));
                            Console.WriteLine(deleted != null ? $"Deleted: {deleted.Name}" : "Not found.");
                        }
                        break;
                    case "Q":
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void StudentMenu()
        {
            Console.WriteLine("\n=== Student Menu ===");
            Console.WriteLine("Select a student:");
            StudentServiceProxy.Current.Students.ForEach(Console.WriteLine);
            Console.Write("Enter Student ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;

            var student = StudentServiceProxy.Current.GetById(id);
            if (student == null) { Console.WriteLine("Student not found."); return; }

            var enrolledCourses = CourseServiceProxy.Current.Courses
                .Where(c => c.Roster.Any(s => s.Id == student.Id)).ToList();

            if (!enrolledCourses.Any())
            {
                Console.WriteLine("You are not enrolled in any courses.");
                return;
            }

            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== Welcome, {student.Name} ===");
                enrolledCourses.ForEach(Console.WriteLine);
                Console.Write("Enter Course ID (or Q to back): ");
                choice = Console.ReadLine();

                if (choice?.ToUpper() == "Q") break;

                if (int.TryParse(choice, out int courseId))
                {
                    var course = enrolledCourses.FirstOrDefault(c => c.Id == courseId);
                    if (course != null)
                        StudentCourseMenu(student, course);
                    else
                        Console.WriteLine("Course not found.");
                }
            } while (true);
        }

        static void StudentCourseMenu(Student student, Course course)
        {
            var choice = string.Empty;
            do
            {
                Console.WriteLine($"\n=== {course.Name} ({course.Code}) ===");
                Console.WriteLine("1. View Modules");
                Console.WriteLine("2. View Assignments");
                Console.WriteLine("3. Submit Assignment");
                Console.WriteLine("4. View My Grades");
                Console.WriteLine("5. View Roster");
                Console.WriteLine("6. View Schedule");
                Console.WriteLine("Q. Back");
                choice = Console.ReadLine();

                switch (choice?.ToUpper())
                {
                    case "1":
                        course.Modules.ForEach(m =>
                        {
                            Console.WriteLine($"\n{m}");
                            m.Content.ForEach(c => Console.WriteLine($"  - {c}"));
                        });
                        break;
                    case "2":
                        course.Assignments.ForEach(Console.WriteLine);
                        break;
                    case "3":
                        SubmitAssignment(student, course);
                        break;
                    case "4":
                        ViewGrades(student, course);
                        break;
                    case "5":
                        course.Roster.ForEach(Console.WriteLine);
                        break;
                    case "6":
                        Console.WriteLine("\n=== Schedule ===");
                        course.Assignments.OrderBy(a => a.DueDate)
                            .ToList().ForEach(a => Console.WriteLine($"{a.DueDate:MM/dd/yyyy} - {a.Name}"));
                        break;
                    case "Q":
                        break;
                }
            } while (!choice.Equals("Q", StringComparison.OrdinalIgnoreCase));
        }

        static void SubmitAssignment(Student student, Course course)
        {
            course.Assignments.ForEach(Console.WriteLine);
            Console.Write("Enter Assignment ID to submit: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) return;
            var assignment = course.Assignments.FirstOrDefault(a => a.Id == id);
            if (assignment == null) { Console.WriteLine("Assignment not found."); return; }

            var submission = new Submission
            {
                Id = assignment.Submissions.Any() ? assignment.Submissions.Max(s => s.Id) + 1 : 1,
                StudentId = student.Id,
                AssignmentId = assignment.Id,
                SubmissionDate = DateTime.Now
            };

            if (assignment.IsQuiz)
            {
                Console.WriteLine("\n=== Quiz ===");
                var answers = new System.Text.StringBuilder();
                for (int i = 0; i < assignment.Questions.Count; i++)
                {
                    var question = assignment.Questions[i];
                    Console.WriteLine($"\nQ{i + 1}: {question.Prompt}");

                    if (question.IsMultipleChoice)
                    {
                        // Show multiple choice options
                        for (int j = 0; j < question.Choices.Count; j++)
                            Console.WriteLine($"  {j + 1}. {question.Choices[j]}");
                        Console.Write("Your answer (enter number): ");
                        var answerIndex = Console.ReadLine();
                        if (int.TryParse(answerIndex, out int idx) && idx > 0 && idx <= question.Choices.Count)
                            answers.AppendLine($"Q{i + 1}: {question.Choices[idx - 1]}");
                        else
                            answers.AppendLine($"Q{i + 1}: {answerIndex}");
                    }
                    else
                    {
                        Console.Write("Your answer: ");
                        answers.AppendLine($"Q{i + 1}: {Console.ReadLine()}");
                    }
                }
                submission.Content = answers.ToString();
            }
            else
            {
                Console.WriteLine("1. Text submission");
                Console.WriteLine("2. File submission");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Your submission: ");
                    submission.Content = Console.ReadLine();
                }
                else if (choice == "2")
                {
                    Console.Write("Enter file path: ");
                    var filePath = Console.ReadLine();
                    if (File.Exists(filePath))
                    {
                        submission.FilePath = filePath;
                        Console.WriteLine("File attached!");
                    }
                    else
                    {
                        Console.WriteLine("File not found, saving path anyway.");
                        submission.FilePath = filePath;
                    }
                }
            }

            assignment.Submissions.Add(submission);
            Console.WriteLine("Submission recorded!");
        }

        static void ViewGrades(Student student, Course course)
        {
            Console.WriteLine($"\n=== Your Grades in {course.Name} ===");
    
            foreach (var assignment in course.Assignments)
            {
                var submission = assignment.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
                if (submission?.Grade != null)
                    Console.WriteLine($"{assignment.Name}: {submission.Grade}/{assignment.AvailablePoints}");
                else
                    Console.WriteLine($"{assignment.Name}: Not graded yet");
            }
            
            //weighted average
            if (course.AssignmentGroups.Any())
            {
                Console.WriteLine("\n=== Weighted Grade ===");
                double totalWeight = 0;
                double weightedScore = 0;

                foreach (var group in course.AssignmentGroups)
                {
                    var groupAssignments = group.Assignments;
                    if (!groupAssignments.Any()) continue;

                    var groupScores = groupAssignments.Select(a =>
                    {
                        var sub = a.Submissions.FirstOrDefault(s => s.StudentId == student.Id);
                        return sub?.Grade != null ? (double)sub.Grade / a.AvailablePoints : 0.0;
                    });

                    double groupAvg = groupScores.Average();
                    weightedScore += groupAvg * group.Weight;
                    totalWeight += group.Weight;
                    Console.WriteLine($"{group.Name}: {groupAvg:P1} (weight: {group.Weight:P0})");
                }
                
                if (totalWeight > 0)
                {
                    double finalGrade = weightedScore / totalWeight * 100;
                    Console.WriteLine($"\nFinal Grade: {finalGrade:F1}%");
                }
            }
            else
            {
                // if no groups then j simple average 
                var gradedSubmissions = course.Assignments
                    .Select(a => a.Submissions.FirstOrDefault(s => s.StudentId == student.Id))
                    .Where(s => s?.Grade != null).ToList();

                if (gradedSubmissions.Any())
                {
                    double avg = gradedSubmissions.Average(s =>
                        (double)s!.Grade! / course.Assignments.First(a => a.Id == s.AssignmentId).AvailablePoints * 100);
                    Console.WriteLine($"\nCurrent Average: {avg:F1}%");
                }
            }
        }
    }
}