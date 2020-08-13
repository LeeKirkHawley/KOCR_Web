
CREATE TABLE Users(Id INTEGER PRIMARY KEY, userName TEXT NOT NULL, pwd TEXT NOT NULL, role TEXT NOT NULL);
CREATE TABLE Documents(userId INTEGER NOT NULL, documentId TEXT NOT NULL, FOREIGN KEY(documentId) REFERENCES Users(rowid));
INSERT INTO Users (userName, pwd, role) VALUES('Kirk', 'pwd', 'Admin');