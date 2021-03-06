drop table if exists businessattributes;
drop table if exists businesscategories;
drop table if exists attributes;
drop table if exists categories;
drop table if exists businesses;
drop table if exists tips;
drop table if exists checkins;
drop table if exists userfriends;
drop table if exists users;

create table users (
	userid varchar not null,
	name varchar null,
	yelpingsince timestamp null,
	averagestars real null,
	fans int null,
	cool int null,
	funny int null,
	useful int null,
	tipcount int null
);

create table userfriends (
	userid varchar not null,
	friendid varchar not null
);

create table businesses (
	businessid varchar not null,
	name varchar null,
	address varchar null,
	city varchar null,
	state varchar null,
	postalcode varchar null,
	latitude real null,
	longitude real null,
	stars real null,
	reviewcount int null,
	isopen int null
);

create table attributes (
	attributeid serial primary key,
	name varchar null
);

create table categories (
	categoryid serial primary key,
	name varchar null
);

create table businessattributes (
	businessid varchar not null,
	attributeid int not null,
	value varchar null
);

create table businesscategories (
	businessid varchar not null,
	categoryid int not null
);

create table checkins (
	businessid varchar not null,
	checkindate timestamp not null
);

create table tips (
	businessid varchar not null,
	userid varchar not null,
	tipdate timestamp not null,
	likes int not null,
	text varchar not null
);
