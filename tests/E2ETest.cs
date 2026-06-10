using lexer.Models;
using parser.Models;
using sqlgenerator.Services;
using transpiler.Services;

namespace tests;

public class E2ETest
{
  [Fact]
  public void Should_Generate_Correct_ProjectionSql()
    {
        string input = "π [name,age] (Students)";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var ast = parser.Parse();
        var sql = new SqlGenerator();
        var result = sql.GenerateSql(ast);
        Assert.Equal(
            "SELECT DISTINCT name, age FROM Students",
            Normalize(result)
        );
    }

    [Fact]
    public void Union_Expression_Should_Generate_Correct_Sql()
    {
        string input = "Students ∪ Teachers";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var ast = parser.Parse();
        var sql = new SqlGenerator();
        var result = sql.GenerateSql(ast);
        Assert.Equal(
            "SELECT * FROM Students UNION SELECT * FROM Teachers",
            Normalize(result)
        );
    }

    [Fact]
    public void Intersection_Expression_Should_Generate_Correct_Sql()
    {
        string input = "Students ∩ Teachers";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var ast = parser.Parse();
        var sql = new SqlGenerator();
        var result = sql.GenerateSql(ast);
        Assert.Equal(
            "SELECT * FROM Students INTERSECT SELECT * FROM Teachers",
            Normalize(result)
        );
    }

    [Fact]
    public void ThetaJoin_Expression_Should_Generate_Correct_Sql()
    {
        string input = "Students ⨝θ[Students.id = Teachers.student_id] Teachers";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var ast = parser.Parse();
        var sql = new SqlGenerator();
        var result = sql.GenerateSql(ast);
        Assert.Equal(
            "SELECT * FROM Students JOIN Teachers ON (Students.id = Teachers.student_id)",
            Normalize(result)
        );
    }
    [Fact]
    public void NaturalJoin_Expression_Should_Generate_Correct_Sql()
    {
        var input ="σ [age > 18] (Student ⋈ Enrollement)";
        var transpiler = new TranspilerService();
        var result = transpiler.Transpile(input);

        var expected = "SELECT * FROM Student NATURAL JOIN Enrollement WHERE (age > 18)";
        Assert.Equal(
            Normalize(expected),
            Normalize(result)
        );
    }
    [Fact]
    public void Multiple_NaturalJoin_Expression_Should_Generate_Correct_Sql()
    {
        var input = "σ [age > 18] (students ⋈ enrolled ⋈ departments)";
        var transpiler = new TranspilerService();
        var result = transpiler.Transpile(input);

        var expected = "SELECT * FROM students NATURAL JOIN enrolled NATURAL JOIN departments WHERE (age > 18)";
        Assert.Equal(
            Normalize(expected),
            Normalize(result)
        );
    }
    [Fact]
    public void Difference_Expression_Should_Generate_Correct_Sql()
    {
        string input = "Students - Teachers";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var ast = parser.Parse();
        var sql = new SqlGenerator();
        var result = sql.GenerateSql(ast);
        Assert.Equal(
            "SELECT * FROM Students EXCEPT SELECT * FROM Teachers",
            Normalize(result)
        );
    }

    [Fact]
    public void Negation_Expression_Should_Generate_Correct_Sql()
    {
        var input = "σ [¬ (dept_id = 1)] (Student)";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var ast = parser.Parse();
        var sqlgenerator = new SqlGenerator();
        var result = sqlgenerator.GenerateSql(ast);
        Assert.Equal(
            "SELECT * FROM Student WHERE NOT (dept_id = 1)",
            Normalize(result)
        );
    }
    [Fact]
    public void Input_WithDecimalValue_Should_Generate_Correct_Sql()
    {
        var input = "σ [gpa>3.5](students)";
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var ast = parser.Parse();
        var sqlgenerator = new SqlGenerator();
        var result = sqlgenerator.GenerateSql(ast);
        Assert.Equal("SELECT * FROM students WHERE (gpa > 3.5)",
            Normalize(result));
    }

    [Fact] 
    public void Transformer_Should_Normalize_SelectionNode()
    {
        var input = " σ [age > 18 ∨ gpa >= 3] (σ [gender='FEMALE'] (σ [height>1.8](students)))";

        var transpiler = new TranspilerService();

        var result = transpiler.Transpile(input);
        var expected = "SELECT * FROM students WHERE (height > 1.8) AND (gender = 'FEMALE') AND (age > 18) OR (gpa >= 3)";

        Assert.Equal(Normalize(expected),Normalize(result));

    }

    [Fact]
    public void Product_Cartesian_Should_Generate_Correct_Sql()
    {
        var input = "students × departments";
        var transpiler = new TranspilerService();

        var result = transpiler.Transpile(input);

        var expected = "SELECT * FROM students, departments";
        Assert.Equal(Normalize(expected),Normalize(result));
    }

    

    private static string Normalize(string result)
    {
        return result
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("  ", " ")
            .Trim();
    }
}
