
CREATE TABLE Users(Id INTEGER PRIMARY KEY, userName TEXT NOT NULL, pwd TEXT NOT NULL, role TEXT NOT NULL);
CREATE TABLE Documents(Id INTEGER PRIMARY KEY, userId INTEGER NOT NULL, documentName TEXT NOT NULL, originalDocumentName TEXT NOT NULL, FOREIGN KEY(userId) REFERENCES Users(ID));
INSERT INTO Users (userName, pwd, role) VALUES('Kirk', 'pwd', 'Admin');