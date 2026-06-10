# Relational Algebra Reference

> **Grammar baseline** — this document is written against the grammar defined in the project.  
> Every section is marked with one of:
> - ✅ **Supported** — within current grammar
> - 🔜 **Planned** — natural next extension
> - 🔬 **Advanced** — requires significant grammar/engine work

---

## Table of Contents

1. [Core Operators (Currently Supported)](#1-core-operators-currently-supported)
   - 1.1 Selection σ
   - 1.2 Projection π
   - 1.3 Rename ρ
   - 1.4 Natural Join ⋈
   - 1.5 Theta Join ⋈θ
2. [Set Operators](#2-set-operators)
   - 2.1 Union ∪
   - 2.2 Intersection ∩
   - 2.3 Difference −
   - 2.4 Cartesian Product ×
3. [Division](#3-division)
4. [Outer Joins](#4-outer-joins)
   - 4.1 Left Outer Join ⟕
   - 4.2 Right Outer Join ⟖
   - 4.3 Full Outer Join ⟗
5. [Extended / Bag Operators](#5-extended--bag-operators)
   - 5.1 Duplicate Elimination δ
   - 5.2 Sorting τ
   - 5.3 Grouping & Aggregation γ
6. [Compound Expressions](#6-compound-expressions)
   - 6.1 Chained Selections
   - 6.2 Chained Projections
   - 6.3 Multi-join Chains
   - 6.4 Full Pipeline

---

## Notation Conventions

| Symbol | ASCII alternative | Meaning |
|--------|-------------------|---------|
| σ | SELECT | Selection (filter rows) |
| π | PROJECT | Projection (pick columns) |
| ρ | RENAME | Rename relation or attributes |
| ⋈ | JOIN | Natural join |
| ⋈θ | JOIN[cond] | Theta join |
| ∪ | UNION | Set union |
| ∩ | INTERSECT | Set intersection |
| − | MINUS / EXCEPT | Set difference |
| × | CROSS | Cartesian product |
| ÷ | DIV | Division |
| ⟕ | LJOIN | Left outer join |
| ⟖ | RJOIN | Right outer join |
| ⟗ | FJOIN | Full outer join |
| δ | DISTINCT | Duplicate elimination |
| τ | SORT | Sorting |
| γ | GROUP | Grouping / aggregation |
| ∧ | AND | Logical and |
| ∨ | OR | Logical or |
| ¬ | NOT | Logical negation |



## 1. Core Operators (Currently Supported)


### 1.1 Selection σ  ✅

Filters rows that satisfy a condition. Never changes the columns.

**Syntax (current grammar)**
```
σ [condition] (Relation)
```

---

#### 1.1.1 Simple equality

```
σ [dept_id = 1] (Student)
```

```sql
SELECT *
FROM Student
WHERE dept_id = 1;
```

---

#### 1.1.2 Inequality

```
σ [age ≠ 20] (Student)
```

```sql
SELECT *
FROM Student
WHERE age <> 20;
```

---

#### 1.1.3 Range comparison

```
σ [gpa >= 3] (Student)
```

```sql
SELECT *
FROM Student
WHERE gpa >= 3;
```

---

#### 1.1.4 Conjunction (AND)

```
σ [age > 18 ∧ gpa >= 3] (Student)
```

```sql
SELECT *
FROM Student
WHERE age > 18
  AND gpa >= 3;
```

---

#### 1.1.5 Disjunction (OR)

```
σ [dept_id = 1 ∨ dept_id = 2] (Student)
```

```sql
SELECT *
FROM Student
WHERE dept_id = 1
   OR dept_id = 2;
```

---

#### 1.1.6 Negation (NOT)

```
σ [¬ (dept_id = 1)] (Student)
```

```sql
SELECT *
FROM Student
WHERE NOT (dept_id = 1);
```

---

#### 1.1.7 Complex condition

```
σ [age > 18 ∧ (gpa >= 3 ∨ dept_id = 2)] (Student)
```

```sql
SELECT *
FROM Student
WHERE age > 18
  AND (gpa >= 3 OR dept_id = 2);
```

---

#### 1.1.8 Selection on string literal

```
σ [grade = 'A'] (Enrolled)
```

```sql
SELECT *
FROM Enrolled
WHERE grade = 'A';
```

---

### 1.2 Projection π  ✅

Picks a subset of columns. In pure relational algebra, duplicates are eliminated (set semantics). In SQL you need DISTINCT to match that.

**Syntax**
```
π [attr1, attr2, ...] (Relation)
```

---

#### 1.2.1 Single attribute

```
π [name] (Student)
```

```sql
SELECT DISTINCT name
FROM Student;
```

---

#### 1.2.2 Multiple attributes

```
π [name, age] (Student)
```

```sql
SELECT DISTINCT name, age
FROM Student;
```

---

#### 1.2.3 Projection after selection

```
π [name, gpa] (σ [age > 18] (Student))
```

```sql
SELECT DISTINCT name, gpa
FROM Student
WHERE age > 18;
```

---

#### 1.2.4 Projection with all attributes (identity)

```
π [id, name, age, dept_id, gpa] (Student)
```

```sql
SELECT DISTINCT id, name, age, dept_id, gpa
FROM Student;
```

---

### 1.3 Rename ρ  ✅

Gives a relation a new name. Useful to disambiguate in self-joins.

**Syntax**
```
ρ NewName (Relation)
```

---

#### 1.3.1 Simple rename

```
ρ S (Student)
```

```sql
SELECT *
FROM Student AS S;
```

---

#### 1.3.2 Rename before join (self-join setup)

```
ρ S1 (Student) ⋈ ρ S2 (Student)
```

```sql
SELECT *
FROM Student AS S1
JOIN Student AS S2
  ON S1.dept_id = S2.dept_id;    -- natural join resolves shared columns
```

---

#### 1.3.3 Rename then select

```
σ [age > 20] (ρ S (Student))
```

```sql
SELECT *
FROM Student AS S
WHERE age > 20;
```

---

### 1.4 Natural Join ⋈  ✅

Joins two relations on **all** columns that share the same name, keeping only one copy of those columns.

**Syntax**
```
R ⋈ S
```

---

#### 1.4.1 Two-table natural join

```
Student ⋈ Enrolled
```

```sql
SELECT *
FROM Student
NATURAL JOIN Enrolled;
```

---

#### 1.4.2 Natural join then project

```
π [name, course_id] (Student ⋈ Enrolled)
```

```sql
SELECT DISTINCT name, course_id
FROM Student
NATURAL JOIN Enrolled;
```

---

#### 1.4.3 Natural join then select

```
σ [grade = 'A'] (Student ⋈ Enrolled)
```

```sql
SELECT *
FROM Student
NATURAL JOIN Enrolled
WHERE grade = 'A';
```

---

#### 1.4.4 Three-table natural join chain

```
Student ⋈ Enrolled ⋈ Course
```

```sql
SELECT *
FROM Student
NATURAL JOIN Enrolled
NATURAL JOIN Course;
```

---

### 1.5 Theta Join ⋈θ  ✅

Joins on an arbitrary condition. Generalises natural join.

**Syntax**
```
R ⋈θ[condition] S
```

---

#### 1.5.1 Equi-join (equality theta)

```
Student ⋈θ[Student.dept_id = Department.id] Department
```

```sql
SELECT *
FROM Student
JOIN Department
  ON Student.dept_id = Department.id;
```

---

#### 1.5.2 Non-equi join

```
Student ⋈θ[Student.gpa > 3] Enrolled
```

```sql
SELECT *
FROM Student
JOIN Enrolled
  ON Student.gpa > 3;
```

---

#### 1.5.3 Theta join with selection

```
σ [grade = 'A'] (Student ⋈θ[Student.id = Enrolled.student_id] Enrolled)
```

```sql
SELECT *
FROM Student
JOIN Enrolled
  ON Student.id = Enrolled.student_id
WHERE grade = 'A';
```

---

#### 1.5.4 Theta join with projection

```
π [name, title] (Student ⋈θ[Student.id = Enrolled.student_id] Enrolled ⋈θ[Enrolled.course_id = Course.id] Course)
```

```sql
SELECT DISTINCT name, title
FROM Student
JOIN Enrolled
  ON Student.id = Enrolled.student_id
JOIN Course
  ON Enrolled.course_id = Course.id;
```

---

## 2. Set Operators

> 🔜 **Planned** — require adding `∪`, `∩`, `−`, `×` to the grammar.  
> Both operands must be **union-compatible** (same arity and attribute types) for ∪, ∩, −.

---

### 2.1 Union ∪

Returns all tuples from either relation (no duplicates).

```
π [dept_id] (Student) ∪ π [dept_id] (Professor)
```

```sql
SELECT DISTINCT dept_id FROM Student
UNION
SELECT DISTINCT dept_id FROM Professor;
```

---

### 2.2 Intersection ∩

Returns only tuples present in **both** relations.

```
π [dept_id] (Student) ∩ π [dept_id] (Professor)
```

```sql
SELECT DISTINCT dept_id FROM Student
INTERSECT
SELECT DISTINCT dept_id FROM Professor;
```

---

### 2.3 Difference −

Returns tuples in the first relation but **not** in the second.

```
π [dept_id] (Student) − π [dept_id] (Professor)
```

```sql
SELECT DISTINCT dept_id FROM Student
EXCEPT
SELECT DISTINCT dept_id FROM Professor;
```

---

### 2.4 Cartesian Product ×

Every tuple from R paired with every tuple from S. Rarely used directly — usually followed by a selection to become a theta join.

```
Student × Department
```

```sql
SELECT *
FROM Student, Department;
-- or equivalently:
SELECT *
FROM Student
CROSS JOIN Department;
```

#### Cartesian product + selection = theta join

```
σ [Student.dept_id = Department.id] (Student × Department)
```

```sql
SELECT *
FROM Student
JOIN Department
  ON Student.dept_id = Department.id;
```

---

## 3. Division

> 🔜 **Planned** — no standard SQL keyword; requires a rewrite rule.

Division R ÷ S answers: *"which values in R are associated with **all** values in S?"*

**Classic use-case:** find students enrolled in **all** courses.

```
π [student_id, course_id] (Enrolled) ÷ π [course_id] (Course)
```

```sql
-- Standard double-negation rewrite:
SELECT DISTINCT student_id
FROM Enrolled e1
WHERE NOT EXISTS (
    SELECT course_id FROM Course
    EXCEPT
    SELECT course_id FROM Enrolled e2
    WHERE e2.student_id = e1.student_id
);
```

---

## 4. Outer Joins

> 🔜 **Planned** — preserves unmatched tuples, filling missing attributes with NULL.

---

### 4.1 Left Outer Join ⟕

Keeps all tuples from the **left** relation; right side is NULL where there is no match.

```
Student ⟕ Enrolled
```

```sql
SELECT *
FROM Student
LEFT JOIN Enrolled
  ON Student.id = Enrolled.student_id;
```

---

### 4.2 Right Outer Join ⟖

Keeps all tuples from the **right** relation.

```
Student ⟖ Enrolled
```

```sql
SELECT *
FROM Student
RIGHT JOIN Enrolled
  ON Student.id = Enrolled.student_id;
```

---

### 4.3 Full Outer Join ⟗

Keeps unmatched tuples from **both** sides.

```
Student ⟗ Enrolled
```

```sql
SELECT *
FROM Student
FULL OUTER JOIN Enrolled
  ON Student.id = Enrolled.student_id;
```

---

## 5. Extended / Bag Operators

> 🔜 / 🔬 — these extend pure relational algebra into the **extended relational algebra** (bag semantics, aggregation, sorting). Required for real SQL coverage.

---

### 5.1 Duplicate Elimination δ

Pure RA has set semantics (no duplicates). SQL uses bag semantics by default. δ maps to `SELECT DISTINCT`.

```
δ (π [dept_id] (Student))
```

```sql
SELECT DISTINCT dept_id
FROM Student;
```

---

### 5.2 Sorting τ

Returns tuples in a specified order. Not part of pure RA (which deals in sets), but essential for SQL output.

```
τ [gpa DESC, name ASC] (Student)
```

```sql
SELECT *
FROM Student
ORDER BY gpa DESC, name ASC;
```

---

### 5.3 Grouping & Aggregation γ

The most powerful extension. Groups tuples and computes aggregate functions.

**Syntax (common notation)**
```
γ [grouping_attrs ; agg_func(attr) → alias] (Relation)
```

#### 5.3.1 Count all rows

```
γ [; COUNT(*) → total] (Student)
```

```sql
SELECT COUNT(*) AS total
FROM Student;
```

#### 5.3.2 Count per group

```
γ [dept_id ; COUNT(*) → num_students] (Student)
```

```sql
SELECT dept_id, COUNT(*) AS num_students
FROM Student
GROUP BY dept_id;
```

#### 5.3.3 Average per group

```
γ [dept_id ; AVG(gpa) → avg_gpa] (Student)
```

```sql
SELECT dept_id, AVG(gpa) AS avg_gpa
FROM Student
GROUP BY dept_id;
```

#### 5.3.4 Multiple aggregates

```
γ [dept_id ; COUNT(*) → n, AVG(gpa) → avg_gpa, MAX(gpa) → max_gpa] (Student)
```

```sql
SELECT dept_id,
       COUNT(*)  AS n,
       AVG(gpa)  AS avg_gpa,
       MAX(gpa)  AS max_gpa
FROM Student
GROUP BY dept_id;
```

#### 5.3.5 Aggregation with HAVING (selection after γ)

```
σ [avg_gpa > 3] (γ [dept_id ; AVG(gpa) → avg_gpa] (Student))
```

```sql
SELECT dept_id, AVG(gpa) AS avg_gpa
FROM Student
GROUP BY dept_id
HAVING AVG(gpa) > 3;
```

---

## 6. Compound Expressions

Complex expressions built from the operators above.  
These are the most important patterns to test your transpiler against as you add features.

---

### 6.1 Chained Selections

```
σ [age > 18] (σ [dept_id = 1] (Student))
```

```sql
-- Two equivalent rewrites:

-- (a) Flatten into one WHERE
SELECT *
FROM Student
WHERE age > 18
  AND dept_id = 1;

-- (b) Subquery (direct structural translation)
SELECT *
FROM (
    SELECT *
    FROM Student
    WHERE dept_id = 1
) AS t
WHERE age > 18;
```

> **Optimisation note:** chained σ should be merged into a single `WHERE` with `AND`.

---

### 6.2 Chained Projections

```
π [name] (π [name, age] (Student))
```

```sql
SELECT DISTINCT name
FROM Student;
```

> **Optimisation note:** outer π subsumes inner π — only the outermost list matters.

---

### 6.3 Multi-join Chain

```
π [name, title, grade]
  (Student
   ⋈[Student.id = Enrolled.student_id] Enrolled
   ⋈[Enrolled.course_id = Course.id] Course)
```

```sql
SELECT DISTINCT name, title, grade
FROM Student
JOIN Enrolled ON Student.id    = Enrolled.student_id
JOIN Course   ON Enrolled.course_id = Course.id;
```

---

### 6.4 Full Pipeline (select → join → project)

```
π [name, title]
  (σ [grade = 'A']
    (Student
     ⋈[Student.id = Enrolled.student_id] Enrolled
     ⋈[Enrolled.course_id = Course.id] Course))
```

```sql
SELECT DISTINCT S.name, C.title
FROM Student S
JOIN Enrolled E ON S.id     = E.student_id
JOIN Course   C ON E.course_id = C.id
WHERE E.grade = 'A';
```

---

### 6.5 Self-join via Rename

Find pairs of students in the same department.

```
π [S1.name, S2.name]
  (σ [S1.dept_id = S2.dept_id ∧ S1.id ≠ S2.id]
    (ρ S1 (Student) × ρ S2 (Student)))
```

```sql
SELECT DISTINCT S1.name, S2.name
FROM Student AS S1
CROSS JOIN Student AS S2
WHERE S1.dept_id = S2.dept_id
  AND S1.id <> S2.id;
```

---

### 6.6 Division pattern (without ÷ operator)

Students who took **every** course:

```
π [student_id, course_id] (Enrolled) ÷ π [course_id] (Course)
```

```sql
SELECT student_id
FROM Student S
WHERE NOT EXISTS (
    SELECT id FROM Course
    EXCEPT
    SELECT course_id FROM Enrolled E
    WHERE E.student_id = S.id
);
```

---

### 6.7 Aggregation after join

```
γ [dept_id ; AVG(gpa) → avg_gpa]
  (σ [grade = 'A']
    (Student ⋈[Student.id = Enrolled.student_id] Enrolled))
```

```sql
SELECT dept_id, AVG(gpa) AS avg_gpa
FROM Student
JOIN Enrolled ON Student.id = Enrolled.student_id
WHERE grade = 'A'
GROUP BY dept_id;
```

---

### 6.8 Nested subquery expressed as RA

Find students whose GPA is above the department average:

```
σ [gpa > avg_gpa]
  (Student
   ⋈[Student.dept_id = D.dept_id]
   (ρ D (γ [dept_id ; AVG(gpa) → avg_gpa] (Student))))
```

```sql
SELECT S.*
FROM Student S
JOIN (
    SELECT dept_id, AVG(gpa) AS avg_gpa
    FROM Student
    GROUP BY dept_id
) AS D ON S.dept_id = D.dept_id
WHERE S.gpa > D.avg_gpa;
```

---


## Implementation Roadmap

Use this as a checklist when extending the grammar:

| Priority | Feature | Grammar addition needed |
|----------|---------|------------------------|
| 1 | **Set union ∪** | `expression "∪" expression` |
| 2 | **Set difference −** | `expression "−" expression` |
| 3 | **Cartesian product ×** | `expression "×" expression` |
| 4 | **Intersection ∩** | `expression "∩" expression` |
| 5 | **Left outer join ⟕** | `expression "⟕" expression` |
| 6 | **Full/right outer join ⟗ ⟖** | same pattern |
| 7 | **Duplicate elimination δ** | `"δ" "(" expression ")"` |
| 8 | **Division ÷** | `expression "÷" expression` |
| 9 | **Sorting τ** | `"τ" "[" sort_list "]" "(" expression ")"` |
| 10 | **Aggregation γ** | `"γ" "[" group_spec "]" "(" expression ")"` — most complex |

Each set operator (1–4) follows the same binary pattern as natural join, so they can be added together in one grammar pass. Outer joins (5–6) are similar. Aggregation (10) requires a new `group_spec` sub-rule with aggregate function syntax.