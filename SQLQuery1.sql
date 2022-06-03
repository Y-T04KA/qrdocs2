create table appdata (
  id int identity(0,1) primary key,
  username varchar(255),
  supervisorname varchar(255),
  adress varchar(255),
  themes varchar(255),
  content varchar(2048),
  resolution varchar(255),
  appstatus tinyint,
  note varchar(512),
  );
 insert into appdata (username,supervisorname,adress,themes,content,appstatus,note)
 values ('Test User Name','Test Supervisor Name','Example Adress','Test Theme','Когда он дает совет, совет этот неожидан, как эпиграмма, и надежен, как английский банк. Я спросил его: «Какая личина скроет меня от мира? Что почтеннее епископов и майоров?» Он повернул ко мне огромное, чудовищное лицо. «Вам нужна надежная маска? – спросил он. – Вам нужен наряд, заверяющий в благонадежности? Костюм, под которым не станут искать бомбы?» Я кивнул. Тогда он зарычал как лев, даже стены затряслись: «Да нарядитесь анархистом, болван! Тогда никто и думать не будет, что вы опасны». Не добавив ни слова, он показал мне широкую спину, а я последовал его совету и ни разу о том не пожалел. Я разглагольствую перед дамами о крови и убийстве, а они, честное слово, дадут мне покатать в колясочке ребёнка',0,'test note');