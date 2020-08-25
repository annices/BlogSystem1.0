USE YourDatabaseName
GO

/* Create table for users. */
CREATE TABLE Users (
userID int primary key identity(1,1),
username VARCHAR(30) unique NOT NULL,
firstname VARCHAR(30),
lastname VARCHAR(30),
mail VARCHAR(50) unique NOT NULL,
password VARCHAR(32)
);

/* Create table for entry categories. */
CREATE TABLE Categories (
categoryID int primary key identity(1,1),
category VARCHAR(50) unique NOT NULL
);

/* Create table for entries. */
CREATE TABLE Entries (
entryID int identity(1,1) primary key,
title varchar(50) NOT NULL,
date DATETIME,
keywords VARCHAR(200),
description VARCHAR(300),
entry VARCHAR(8000) NOT NULL,
categoryID int NOT NULL,
	CONSTRAINT fk_category FOREIGN KEY (categoryID) REFERENCES Categories(categoryID)
	ON DELETE CASCADE,
userID int NOT NULL,
	CONSTRAINT fk_username FOREIGN KEY (userID) REFERENCES Users(userID)
    ON DELETE CASCADE
);

/* Create table for entry comments. */
CREATE TABLE Comments (
commentID int identity(1,1) primary key,
name VARCHAR(30),
mail VARCHAR(50) NOT NULL,
date DATETIME,
website VARCHAR(100),
comment VARCHAR(300) NOT NULL,
answer VARCHAR(300),
entryID int NOT NULL,
	CONSTRAINT fk_entryID FOREIGN KEY (entryID) REFERENCES Entries(entryID)
	ON DELETE CASCADE
);

/* Insert default values for a user. */
INSERT Users VALUES ('YourUserName', 'YourFirstname', 'YourLastname', 'yourmail@mail.se', HASHBYTES('MD5', 'YourPassword'));

/* Insert sample data for categories. */
INSERT Categories VALUES ('Allmänt');
INSERT Categories VALUES ('Data & IT');
INSERT Categories VALUES ('Musik');

/* Insert sample data for entries. */
INSERT Entries VALUES ('Lorem ipsum', '2016-11-04', 'Lorem, ipsum.', 'Ett lorem ipsum-inlägg.',
'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.',
1, 1);
INSERT Entries VALUES ('Data och IT', '2016-10-04', 'Data, IT, internet.', 'Ett inlägg om data och IT.',
'Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?',
2, 1);
INSERT Entries VALUES ('Musikinlägg', '2016-09-15', 'Musik.', 'Ett inlägg om musik.',
'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.',
3, 1);

/* Insert sample data for comments. */
INSERT Comments (name, mail, date, comment, entryID) VALUES ('Gäst', 'gastmail@mail.se', '2016-11-04', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.', 1);
INSERT Comments (name, mail, date, website, comment, entryID) VALUES ('Gäst2', 'gast2@mail.se', '2016-11-06', 'http://gasthemsida.se', 'Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet.', 1);
INSERT Comments (mail, date, comment, entryID) VALUES ('gast3@mail.se', '2016-11-06', 'Lorem ipsum dolor sit amet.', 1);