

CREATE TABLE appdata (
    [id]             INT            IDENTITY (0, 1) NOT NULL,
    [username]       VARCHAR (255)  NOT NULL,
    [supervisorname] VARCHAR (255)  NOT NULL,
    [adress]         VARCHAR (255)  NOT NULL,
    [themes]         VARCHAR (255)  NOT NULL,
    [content]        VARCHAR (2048) NOT NULL,
    [resolution]     VARCHAR (255)  NULL,
    [appstatus]      TINYINT        NOT NULL,
    [note]           VARCHAR (512)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);