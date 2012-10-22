drop table if exists properties;
create table if not exists properties (
	property text primary key, 
	value text not null
);

drop table if exists players;
create table if not exists players (
	id integer primary key autoincrement,
	name text not null
);	