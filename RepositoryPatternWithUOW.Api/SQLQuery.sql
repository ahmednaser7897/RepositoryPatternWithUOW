/*

Departments
    │
    ├── 1 : M Employees
    ├── 1 : M Instructors

So your seed script should insert in this order:

Departments → Employees

*/

-- =========================================================
-- Departments
-- =========================================================

INSERT INTO Departments (Name, ManagerName)
VALUES
('CS', 'Ahmed'),
('IT', 'Ali'),
('HR', 'Hassan'),
('Finance', 'Mahmoud');


-- =========================================================
-- Employees
-- =========================================================

INSERT INTO Employees
    (Name, Salary, Address, ImageUrl, JopTitle, DepartmentId)
VALUES

-- CS
('John', 1000, '123 Main St', '2.png', 'BackEnd Developer',
    (SELECT Id FROM Departments WHERE Name = 'CS')),

('Ahmed', 1500, '456 Second St', '1.png', 'FrontEnd Developer',
    (SELECT Id FROM Departments WHERE Name = 'CS')),

('Sara', 1200, '789 Third St', '3.png', 'BackEnd Developer',
    (SELECT Id FROM Departments WHERE Name = 'CS')),

-- IT
('Omar', 1300, '101 Fourth St', '4.png', 'Backend Developer',
    (SELECT Id FROM Departments WHERE Name = 'IT')),

('Mona', 1600, '202 Fifth St', '5.png', 'Frontend Developer',
    (SELECT Id FROM Departments WHERE Name = 'IT')),

('Youssef', 1400, '303 Sixth St', '6.png', 'DevOps Engineer',
    (SELECT Id FROM Departments WHERE Name = 'IT')),

-- HR
('Khaled', 1100, '404 Seventh St', '7.png', 'HR Specialist',
    (SELECT Id FROM Departments WHERE Name = 'HR')),

('Nour', 1250, '505 Eighth St', '8.png', 'Recruiter',
    (SELECT Id FROM Departments WHERE Name = 'HR')),

('Mai', 1350, '606 Ninth St', '9.png', 'HR Manager',
    (SELECT Id FROM Departments WHERE Name = 'HR')),

-- Finance
('Mostafa', 1800, '707 Tenth St', '10.png', 'Accountant',
    (SELECT Id FROM Departments WHERE Name = 'Finance')),

('Dina', 2000, '808 Eleventh St', '11.png', 'Financial Analyst',
    (SELECT Id FROM Departments WHERE Name = 'Finance')),

('Karim', 2200, '909 Twelfth St', '12.png', 'Finance Manager',
    (SELECT Id FROM Departments WHERE Name = 'Finance'));