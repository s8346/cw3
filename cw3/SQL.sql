Alter procedure PromoteStudents @studies varchar(32), @semester int
as
begin
declare @idenrollmentnew int,
@idenrollmentold int,
@studiesid int;
select @studiesid=IdStudy from Studies where Name=@studies;
if @studiesid is not null
begin
select @idenrollmentold=IdEnrollment from Enrollment where IdStudy=@studiesid and Semester=@semester;
select @idenrollmentnew=IdEnrollment from Enrollment where IdStudy=@studiesid and Semester=@semester+1;
if @idenrollmentnew is not null
    begin
    update Student set IdEnrollment=@idenrollmentnew where IdEnrollment=@idenrollmentold;
    end
else
    begin
    declare @nextid int
    select @nextid=MAX(IdEnrollment)+1 from Enrollment;
    insert into Enrollment (IdEnrollment, Semester, IdStudy, StartDate) values (@nextid, @semester+1,@studiesid,GETDATE());
    update Student set IdEnrollment=@nextid where IdEnrollment=@idenrollmentold;
    end
end
else
begin
    raiserror('Blad',1,2)
end
end