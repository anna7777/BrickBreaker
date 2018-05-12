 create table Map
 (id int identity,
  title varchar(250))
  
  insert into Map(title)
  values ('C  l  a  s  s  i  c'), ('C  h  e  s  s'), ('C  u  p  l  e  s'), ('R  a  i  l  s'), ('K  e  y  s'), ('A  b  s  t  r  a  c  t  i  o  n')
  
  create table Brick
  (id int identity,
   map_id int,
   x int,
   y int)
   
  insert into Brick(map_id, x, y)
  values (1, 0, 0), (1, 0, 1), (1, 0, 2), (1, 0, 3), (1, 0, 4), (1, 0, 5), (1, 0, 6),
		 (1, 1, 1), (1, 1, 2), (1, 1, 3), (1, 1, 4), (1, 1, 5),
		 (1, 2, 2), (1, 2, 3), (1, 2, 4),
		 
		 (2, 0, 0), (2, 0, 2), (2, 0, 4), (2, 0, 6),
		 (2, 1, 1), (2, 1, 3), (2, 1, 5),
		 (2, 2, 0), (2, 2, 2), (2, 2, 4), (2, 2, 6),
		 
		 (3, 0, 1), (3, 0, 3), (3, 0, 5),
		 (3, 1, 0), (3, 1, 1), (3, 1, 2), (3, 1, 3), (3, 1, 4), (3, 1, 5), (3, 1, 6),
		 (3, 2, 0), (3, 2, 2), (3, 2, 4), (3, 2, 6),
		 
		 (4, 0, 0), (4, 0, 1), (4, 0, 2), (4, 0, 3), (4, 0, 4), (4, 0, 5), (4, 0, 6),
		 (4, 1, 0), (4, 1, 2), (4, 1, 4), (4, 1, 6),
		 (4, 2, 0), (4, 2, 1), (4, 2, 2), (4, 2, 3), (4, 2, 4), (4, 2, 5), (4, 2, 6),
		 
		 (5, 0, 0), (5, 0, 3), (5, 0, 4), (5, 0, 6),
		 (5, 1, 0), (5, 1, 1), (5, 1, 2), (5, 1, 3), (5, 1, 4), (5, 1, 5), (5, 1, 6),
		 (5, 2, 0), (5, 2, 2), (5, 2, 3), (5, 2, 6),
		 
		 (6, 0, 0), (6, 0, 2), (6, 0, 3), (6, 0, 4), (6, 0, 5), (6, 0, 6),
		 (6, 1, 1), (6, 1, 3), (6, 1, 5),
		 (6, 2, 0), (6, 2, 1), (6, 2, 2), (6, 2, 3), (6, 2, 4), (6, 2, 6) 
		 
ALTER TABLE	Map ADD CONSTRAINT pk1 PRIMARY KEY NONCLUSTERED(id)
ALTER TABLE	Brick ADD CONSTRAINT pk2 PRIMARY KEY NONCLUSTERED(id)

ALTER TABLE	Brick ADD CONSTRAINT fk1 FOREIGN KEY(map_id) REFERENCES Map(id)
		 
select x,y,title
from Brick, Map
where map_id = Map.id

--drop table Brick
--drop table Map