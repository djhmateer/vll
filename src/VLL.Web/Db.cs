﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace VLL.Web
{
	public record Dashboard500VM(
		DateTime DateTimeUtc,
		string Path,
		string? Email
	);

	public record Dashboard404VM(
		DateTime DateTimeUtc,
		string IPAddress,
		string Path,
		string? UserAgent,
		string? Email
	);


	public record DashboardLoginAndJob(
		   string? Email, // make mapping easier by keeping this nullable
		   DateTime DateTimeUtcJobStartedOnVM,
		   int JobTypeId,
		   string OrigFileName,
		   int TimeTakenInS
	   );


	public record DashboardRealPage(
		   DateTime DateTimeUtc,
		   string IPAddress,
		   string Path,
		   string? UserAgent,
		   string? Email
	);

	public record DashboardRequest(
		int WebLogId,
		int WebLogTypeId,
		DateTime DateTimeUtc,
		string? IpAddress,
		string Verb,
		string Path,
		string? QueryString,
		int StatusCode,
		int ElapsedTimeInMs,
		string? Referer,
		bool? DisplayReferer,
		string? UserAgent,
		string HttpVersion,
		int? LoginId,
		string? Email,
		string? RoleName
	);

	public record Login(
		int LoginId,
		string Email,
		string PasswordHash,
		int? RoleId,
		int LoginStateId,
		int PasswordFailedAttempts,
		Guid? PasswordResetVerificationCode,
		DateTime? PasswordResetVerificationSentDateTimeUtc,
		int MfaFailedAttempts,
		int? MfaCode,
		DateTime? MfaSentDateTimeUtc,
		Guid EmailAddressConfirmationCode,
		DateTime DateTimeUtcCreated
		);

	public record LoginSmall(
		// not nullable in db, but useful for this concept of initiating
		int? LoginId,
		string Email,
		string PasswordHash,
		int LoginStateId,
		// nice default set here
		int? RoleId = null

	 );


	public record LoginMemberVM(
		int LoginId,
		string Email
		);



	public record LoginAdminViewModel(
		// not nullable in db, but useful for this concept of initiating
		int? LoginId,
		string Email,
		string PasswordHash,
		int LoginStateId,
		string LoginStateName,
		// nice default set here
		string? RoleName,
		int? RoleId = null
	);

	public record LoginState(
		int LoginStateId,
		string Name
	);


	public record Job(
		int JobId,
		int LoginId,
		string OrigFileName,
		DateTime DateTimeUtcUploaded,
		int? JobStatusId,
		int? VMId,
		DateTime? DateTimeUtcJobStartedOnVm,
		DateTime? DateTimeUtcJobEndedOnVm,
		int JobTypeId
	);

	public record JobViewModel(
		int JobId,
		int LoginId,
		string OrigFileName,
		DateTime DateTimeUtcUploaded,
		int? JobStatusId,
		string? JobStatusString,
		int? VMId,
		DateTime? DateTimeUtcJobStartedOnVm,
		DateTime? DateTimeUtcJobEndedOnVm,
		int JobTypeId,
		string? JobType
	);

	public record LogSmall(
		int LogId,
		string Text,
		DateTime DateTimeUtc
	);

	public record OSREmail(
		string ToEmailAddress,
		string Subject,
		string TextBody,
		string HtmlBody
	);

	// HERE

	public record ProjectViewModel(
	 // not nullable in db, but useful for this concept of initiating
	 int? ProjectId,
	 string Name,
	 DateTime DateTimeCreatedUtc,
	 string ShortDescription

	);

	public record IssueViewModel(
	// not nullable in db, but useful for this concept of initiating
	int? IssueId,
	string Name,
	string Description,
	string? RegulatorName
	);

	// used by /projects
	public record ProjectFullViewModel(
 int? ProjectId,
 string Name,
 int ProjectStatusId,
 bool IsPublic,
 int PromoterLoginId,
 string? ShortDescription,
 string? Description,
 string? Keywords,
 string? ResearchNotes,
 DateTime DateTimeCreatedUtc,
 string? PromoterEmail
);

	// used by /project/4
	public record ProjectAllTablesViewModel(
 int? ProjectId,
 string Name,
 int ProjectStatusId,
 bool IsPublic,
 int PromoterLoginId,
 string? ShortDescription,
 string? Description,
 string? Keywords,
 string? ResearchNotes,
 DateTime DateTimeCreatedUtc,
 string? PromoterEmail,
 string? ProjectStatusName
);

	// used by /project/4
	public record ProjectMembersViewModel(
 int? LoginId,
 string Email,
 string? InstitutionName,
 string? ProfileUrl
);

	// used by /account/manage/1
	public record LogindAndPerson(
 int? LoginId,
 string Email,
 string? ContactNumber,
 string? InstitutionName,
 string? ProfileUrl,
 string? Name
);

	// used by /project/4
	public record ProjectLinksViewModel(
 int? LinkId,
 int ProjectId,
 string Url,
 string? Description
);

	// used by /project/4
	public record ProjectIssueViewModel(
 int? IssueId,
 string Name,
 string? Description,
 string? RegulatorName
);

	// used by /project/edit?projectId=4
	public record ProjectEditViewModel(
 //int? ProjectId,
 int ProjectId,
 string Name,
 int ProjectStatusId,
 bool IsPublic,
 int? PromoterLoginId,
 string? ShortDescription,
 string? Description,
 string? Keywords,
 string? ResearchNotes,
 DateTime DateTimeCreatedUtc
);

	// used by /issue/edit?issueId=2
	public record IssueEditViewModel(
int? IssueId,
int ProjectId,
int? RegulatorId,
string Name,
int IssueStatusId,
bool IsPublic,
string? Description,
string? Keywords,
string? Response,
DateTime DateTimeCreatedUtc,
string? ProjectName
//string? RegulatorName
);


	// used by /issue/2
	public record IssueAllTablesViewModel(
int? IssueId,
int ProjectId,
int? RegulatorId,
string Name,
int IssueStatusId,
bool IsPublic,
string? Description,
string? Keywords,
string? Response,
DateTime DateTimeCreatedUtc,
string? ProjectName,
string? RegulatorName,
string IssueStatusName
);

	public record ProjectStatus(
	   int ProjectStatusId,
	   string Name
   );

	public record IssueStatus(
   int IssueStatusId,
   string Name
);

	// used as a dropdown in /project/edit
	public record PromoterLogin(
   int LoginId,
   string Email
);

	public record Regulator(
int RegulatorId,
string Name,
string? ContactEmail
);


	public static class LoginStateId
	{
		public const int WaitingToBeInitiallyVerifiedByEmail = 1;
		public const int InUse = 2;
		public const int PasswordResetSent = 3;
		public const int LockedOutDueTo3WrongPasswords = 4;

		public const int Disabled = 99;
	}

	public static class RoleId
	{
		// Registered but email not manually verified yet
		// can use the application in a limited way
		public const int Tier1 = 1;

		// Email has been manually verified
		// Can use the application fully
		public const int Tier2 = 2;

		public const int Admin = 9;
	}

	public static class CDRole
	{
		public const string Tier1 = "Tier1";
		public const string Tier2 = "Tier2";
		public const string Admin = "Admin";
	}

	public static class Db
	{
		public static IDbConnection GetOpenConnection(string connectionString)
		{
			if (connectionString == null) throw new ArgumentException("ConnectionString can't be null");
			DbConnection cnn = new SqlConnection(connectionString);
			return cnn;
		}

		public static async Task<LoginSmall?> GetLoginByEmail(string connectionString, string email)
		{
			using var conn = GetOpenConnection(connectionString);

			// emails in the db may be in upper and lower case
			// we don't allow dupe records in our db
			// davemateer@gmail.com
			// DaveMateer@gmail.com

			// so lower whatever is coming in
			email = email.ToLower();

			var result = await conn.QueryAsyncWithRetry<LoginSmall?>(@"
                select LoginId, Email, PasswordHash, LoginStateId, RoleId
                from login
                -- @Email is lower, and we are lowering whatever is in the Db
                where LOWER(email) = @Email
                ", new { email });

			result = result.ToArray();

			if (result.Count() > 1)
				throw new ApplicationException("Cannot have duplicate emails in database");

			return result.SingleOrDefault();
		}

		public static async Task IncrementNumberOfFailedLoginsForEmailLogin(string connectionString, string email)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
              update Login 
              set PasswordFailedAttempts = PasswordFailedAttempts + 1
              where Email = @Email 
              ", new { email });
		}

		public static async Task<bool> CheckIfNeedToLockAccountForEmailLogin(string connectionString, string email)
		{
			using var conn = GetOpenConnection(connectionString);

			var numberOfFailedLogins = await conn.QueryAsyncWithRetry<int>(@"
                select PasswordFailedAttempts
                from Login
                where Email = @Email
                ", new { email });

			var foo = numberOfFailedLogins.First();

			if (foo > 3)
			{
				var lockedOut = LoginStateId.LockedOutDueTo3WrongPasswords;
				await conn.ExecuteAsyncWithRetry(@"
                    update login
                    set LoginStateId = @LockedOut
                    where Email = @Email
                    ", new { lockedOut, email });
				return true;
			}

			return false;
		}

		public static async Task ResetFailedLoginsForEmailLogin(string connectionString, string email)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update login
                set PasswordFailedAttempts = 0
                where Email = @Email
                ", new { email });
		}

		public static async Task<Login> InsertLogin(string connectionString, LoginSmall login)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Login>(@"
                insert Login(Email, PasswordHash, LoginStateId)
                output inserted.*
                values(@Email, @PasswordHash, @LoginStateId)
                ", login);

			return result.First();
		}

		public static async Task<int> InsertJobWithOrigFileNameAndReturnJobId(string connectionString, int loginId, string origFileName, int jobTypeId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
                insert into Job (LoginId, OrigFileName, DateTimeUtcUploaded, JobStatusId, JobTypeId)
                output inserted.JobId
                values (@LoginId, @OrigFileName, GETUTCDATE(), @JobStatusId, @JobTypeId)
                ", new { loginId, origFileName, jobStatusId = JobStatusId.WaitingToStart, jobTypeId });

			return result.Single();
		}

		public static class JobTypeId
		{
			public const int FaceSearch = 1;
			public const int HateSpeech = 2;
			public const int SpeechParts = 3;
		}

		public static class JobStatusId
		{
			public const int WaitingToStart = 1;
			public const int Running = 2;
			public const int Completed = 3;
			public const int CancelledByUser = 4;
			public const int Exception = 9;
		}

		public static class VMStatusId
		{
			public const int CreatingVM = 1;
			public const int ReadyToRunJobOnVM = 2;
			public const int RunningJobOnVM = 3;
			public const int DeletingVM = 4;
			public const int Deleted = 5;
		}

		public static class VMTypeId
		{
			public const int FaceSearchGPU = 1;
			public const int HateSpeechCPU = 2;
			public const int SpeechPartsCPU = 3;
		}

		public static async Task UpdateJobIdToStatusId(string connectionString, int jobId, int jobStatusId)
		{
			if (jobId != 0)
			{
				using var conn = GetOpenConnection(connectionString);

				await conn.ExecuteAsyncWithRetry(@"
                update Job
                set JobStatusId = @JobStatusId
                where JobId = @JobId
                ", new { jobId, jobStatusId });
				return;
			}

			Log.Warning($"{nameof(UpdateJobIdToStatusId)} has an jobId of 0 passed to it. Could be because of an Exception in a FileProcessingService");
		}

		public static async Task UpdateJobToStatusCompleted(string connectionString, int jobId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update Job
                set JobStatusId = @JobStatusId
                where JobId = @JobId
                ", new { jobId, jobStatusId = JobStatusId.Completed });
		}

		public static async Task<int> GetJobStatusId(string connectionString, int jobId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
                select JobStatusId
                from Job
                where JobId = @JobId
                ", new { jobId });

			return result.Single();
		}

		public static async Task<List<LogSmall>> GetLogsForJobId(string connectionString, int jobId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<LogSmall>(@"
                select LogId, [Text], DateTimeUtc
                from Log
                where JobId = @JobId
                order by DateTimeUtc desc
                ", new { jobId });

			return result.ToList();
		}

		// normal log insert
		public static async Task InsertLog(string connectionString, int jobId, string text)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                insert into Log(JobId, [Text])
                values(@JobId, @Text)
                ", new { jobId, text });

			// should never happen except on very exceptional circumstances
			if (jobId == 0) Log.Warning($"{nameof(InsertLog)} inserted a log into the db with jobId 0 and text: {text}.");
		}

		// Used by the event coming back from remote server
		public static void InsertLogNotAsyncWithRetry(string connectionString, int jobId, string text)
		{
			using var conn = GetOpenConnection(connectionString);

			conn.ExecuteWithRetry(@"
                insert into Log(JobId, [Text])
                values(@JobId, @Text)
                ", new { jobId, text });
		}

		// only used by polly-test to get SQL Azure to fail
		//public static void InsertLogNotAsync(string connectionString, int jobId, string text)
		//{
		//    using var conn = GetOpenConnection(connectionString);

		//    conn.Execute(@"
		//    insert into Log(JobId, [Text])
		//    values(@JobId, @Text)
		//    ", new { jobId, text });
		//}

		public static async Task<VMFromDb?> GetFreeVMIfExists(string connectionString, int vmTypeId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<VMFromDb?>(@"
                select * 
                from VM
                where VMStatusId = 2 -- ReadyToReceiveJobOnVM
                and VMTypeId = @VMTypeId
                ", new { vmTypeId });

			return result.SingleOrDefault();
		}


		public record VMFromDb(int VMId, int VMStatusId, string? ResourceGroupName, DateTime DateTimeUtcCreated,
			DateTime? DateTimeUtcDeleted, string Password, int VMTypeId);

		public static async Task<VMFromDb> CreateNewVM(string connectionString, string passwordVM, int vmTypeId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
                insert into VM (VMStatusId, DateTimeUtcCreated, Password, VMTypeId)
                output inserted.VMId as VMId
                values (1, GETUTCDATE(), @PasswordVM, @VMTypeId)
                ", new { passwordVM, vmTypeId });

			var vmId = result.Single();

			// The Id of the VM is the same as the DB assigned identity seed
			string resourceGroupName;

			if (vmTypeId == VMTypeId.FaceSearchGPU)
				resourceGroupName = $"webfacesearchgpu{vmId}";

			else if (vmTypeId == VMTypeId.HateSpeechCPU)
				resourceGroupName = $"webhatespeechcpu{vmId}";

			else if (vmTypeId == VMTypeId.SpeechPartsCPU)
				resourceGroupName = $"speechpartscpu{vmId}";

			else
				throw new ApplicationException("Unexpected VMTypeId - check in CreateNewVM Method");

			var foo = await conn.QueryAsyncWithRetry<VMFromDb>(@"
                update VM
                set ResourceGroupName = @ResourceGroupName
                where VMId = @VMId

                select *
                from VM
                where VMId = @VMId
                ", new { vmId, resourceGroupName });

			return foo.Single();
		}

		public static async Task UpdateVMStatusId(string connectionString, int vmId, int vmStatusId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update VM
                set VMStatusId = @VMStatusID
                where VMId = @VMId
                ", new { vmId, vmStatusId });

			// should never happen
			if (vmId == 0) Log.Warning($"{nameof(UpdateVMStatusId)} has an vmId of 0 passed to it.");
		}

		public static async Task UpdateJobVMIdDetails(string connectionString, int jobId, int vmId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update Job
                set VMId = @VMId,
                    DateTimeUtcJobStartedOnVM = GETUTCDATE()
                where JobId = @JobId
                ", new { jobId, vmId });
		}

		public static async Task<int> GetCountOfAllVMs(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
                select count(*) from VM
                ");

			// should always be something, but just in case there are no records in VM return 0
			return result.SingleOrDefault();
		}

		public static async Task UpdateLoginEmailAddressConfirmationCode(string connectionString, int loginId,
			Guid guid)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update Login
                set EmailAddressConfirmationCode = @Guid
                where LoginId = @LoginId
                ", new { loginId, guid });
		}

		public static async Task DeleteLoginWithEmail(string connectionString, string email)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                delete from login
                where email = @Email
                ", new { email });
		}

		public static async Task<Login?> GetLoginByEmailConfirmationCode(string connectionString,
			Guid emailAddressConfirmationCode)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Login?>(@"
                select * from login
                where EmailAddressConfirmationCode = @EmailAddressConfirmationCode
                    and GETUTCDATE() < dateadd(hh, 1, DateTimeUtcCreated)
                    and loginStateId = @LoginStateId
                ",
				new { emailAddressConfirmationCode, loginStateId = LoginStateId.WaitingToBeInitiallyVerifiedByEmail });

			return result.SingleOrDefault();
		}

		public static async Task UpdateLoginIdWithLoginStateId(string connectionString, int loginId, int loginStateId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update login
                set loginStateId = @LoginStateId
                where loginId = @LoginId
                ", new { loginId, loginStateId });
		}

		public static async Task UpdateLoginIdWithRoleId(string connectionString, int loginId, int roleId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update login
                set RoleId = @RoleId
                where loginId = @LoginId
                ", new { loginId, roleId });
		}

		public static async Task UpdateLoginIdForgotPasswordResetWithTimeAndGuid(string connectionString, int loginId,
			Guid guid)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update login
                set LoginStateId = 3, -- Password Reset Code Sent 
                    PasswordResetVerificationCode = @Guid,
                    PasswordResetVerificationSentDateTimeUtc = GETUTCDATE()
                where loginId = @LoginId
                ", new { loginId, guid });
		}

		public static async Task<LoginSmall?> GetLoginByPasswordResetVerificationCode(string connectionString,
			Guid guid)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<LoginSmall?>(@"
                select LoginId, Email, PasswordHash, LoginStateId, RoleId
                from login
                where PasswordResetVerificationCode = @Guid
                  and GETUTCDATE() < dateadd(hh, 1, PasswordResetVerificationSentDateTimeUtc)
                ", new { guid });

			return result.FirstOrDefault();
		}

		public static async Task UpdateLoginPasswordAndResetFailedLoginsAndVerificationCode(string connectionString,
			int loginId, string newPasswordHash)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update Login 
                set 
                    PasswordHash = @NewPasswordHash,
                    LoginStateId = 2, -- InUse
                    PasswordFailedAttempts = 0,
                    PasswordResetVerificationCode = null,
                    PasswordResetVerificationSentDateTimeUtc = null
                where LoginId = @LoginId
                ", new { loginId, newPasswordHash });

		}

		public static async Task UpdateLoginIdSetEmailAddressConfirmationCodeToNull(string connectionString,
			int loginId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update Login 
                set 
                    EmailAddressConfirmationCode = null
                where LoginId = @LoginId
                ", new { loginId });
		}

		public static async Task<List<LoginSmall>> GetAllLogins(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<LoginSmall>(@"
                select LoginId, Email, PasswordHash, LoginStateId, RoleId
                from login
                ");

			return result.ToList();
		}

		public static async Task UpdateJobIdDateTimeUtcJobEndedOnVM(string connectionString, int jobId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update job
                set DateTimeUtcJobEndedOnVM = GETUTCDATE()
                where jobId = @JobId
                ", new { jobId });
		}

		public static async Task UpdateVMDateTimeUtcDeletedToNow(string connectionString, int vmId)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                update vm
                set DateTimeUtcDeleted = GETUTCDATE()
                where vmId = @VmId
                ", new { vmId });

			// should only happen under unusual circumstances eg a catch/finally from a service 
			if (vmId == 0) Log.Warning($"{nameof(UpdateVMDateTimeUtcDeletedToNow)} has an vmId of 0 passed to it.");
		}

		public static async Task<List<Job>> GetJobsForLoginId(string connectionString, int loginId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Job>(@"
                select *
                from Job
                where LoginId = @LoginId
                ", new { loginId });

			return result.ToList();
		}

		public static async Task<bool> CheckIfLoginIdIsAllowedToViewThisJobId(string connectionString, int loginId, int jobId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<bool>(@"
                select count(*) 
                from Job
                where LoginId = @LoginId
                    and JobId = @JobId
                ", new { loginId, jobId });

			// https://stackoverflow.com/a/31282196/26086
			// When  0 is returned Dapper will return False
			return result.FirstOrDefault();
		}

		public static async Task<Job> GetJobByJobId(string connectionString, int jobId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Job>(@"
                select *
                from Job
                where JobId = @JobId
                ", new { jobId });

			// Want it to throw if none found
			return result.Single();
		}

		public static async Task<int> GetVmIdByResourceGroupName(string connectionString, string resourceGroupName)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
                select VmId
                from Vm
                where ResourceGroupName = @ResourceGroupName
                ", new { resourceGroupName });

			return result.Single();
		}

		public static async Task<string> GetEmailByJobId(string connectionString, int jobId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<string>(@"
                select l.Email
                from Login l
                join Job j on j.LoginId = l.LoginId
                where j.JobId = @JobId
                ", new { jobId });

			return result.Single();
		}

		public static async Task<List<int>> GetVmIdsStatus2ReadyToRunJob(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
                select VMid
                from VM
                where VMStatusId = 2
                ");

			return result.ToList();
		}

		public static async Task<DateTime?> GetMostRecentLogDateTimeUtcForMostRecentJobRunningOnVmId(string connectionString, int vmId)
		{
			using var conn = GetOpenConnection(connectionString);

			// will get all jobs which have been run on the VM (if multiple)
			// only return the last log entry
			var result = await conn.QueryAsyncWithRetry<DateTime?>(@"
                select top 1 l.DateTimeUtc
                from Log l
                join Job j on j.JobId = l.JobId
                where VMId = @VMId
                order by l.DateTimeUtc desc
                ", new { vmId });

			// if no log entry, send back default
			return result.SingleOrDefault();
		}

		public static async Task<Job?> GetMostRecentJobOnVmId(string connectionString, int vmId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Job?>(@"
                select top 1 * 
                from Job 
                where VMId = @VMId
                order by DateTimeUtcJobStartedOnVM desc
                ", new { vmId });

			return result.SingleOrDefault();
		}

		public static async Task<List<LoginState>> GetAllLoginStates(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<LoginState>(@"
                select * 
                from LoginState 
                order by LoginStateId 
                ");

			return result.ToList();
		}

		public static async Task<LoginSmall> GetLoginByLoginId(string connectionString, int loginId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<LoginSmall>(@"
                select LoginId, Email, PasswordHash, LoginStateId, RoleId
                from login
                where loginId = @LoginId
                ", new { loginId });

			return result.Single();
		}

		public static async Task UpdateLoginStateIdAndRoleIdByLoginId(string connectionString, int loginId, int loginStateId, int? roleId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.ExecuteAsyncWithRetry(@"
                update login
                set LoginStateId = @LoginStateId,
                    RoleId = @RoleId
                where loginId = @LoginId
                ", new { loginId, loginStateId, roleId });
		}

		// used by startup.cs to insert into custom weblog table
		// this should be a fast insert!
		public static async Task InsertWebLog(string connectionString,
			int webLogTypeId,
			string? ipAddress,
			string verb,
			string path,
			string? queryString,
			int statusCode,
			int elapsedTimeInMs,
			string? referer,
			string? userAgent,
			string httpVersion,
			int? loginId,
			string? email,
			string? roleName)
		{
			using var conn = GetOpenConnection(connectionString);

			await conn.ExecuteAsyncWithRetry(@"
                INSERT INTO [dbo].[WebLog]
                       ([WebLogTypeId]
                       ,[DateTimeUtc]
                       ,[IPAddress]
                       ,[Verb]
                       ,[Path]
                       ,[QueryString]
                       ,[StatusCode]
                       ,[ElapsedTimeInMs]
                       ,[Referer]
                       ,[UserAgent]
                       ,[HttpVersion]
                       ,[LoginId]
                       ,[Email]
                       ,[RoleName])
                 VALUES
                       (@WebLogTypeId
                       ,GETUTCDATE()
                       ,@IPAddress
                       ,@Verb
                       ,@Path
                       ,@QueryString
                       ,@StatusCode
                       ,@ElapsedTimeInMs
                       ,@Referer
                       ,@UserAgent
                       ,@HttpVersion
                       ,@LoginId
                       ,@Email
                       ,@RoleName)
            ", new
			{
				webLogTypeId,
				ipAddress,
				verb,
				path,
				queryString,
				statusCode,
				elapsedTimeInMs,
				referer,
				userAgent,
				httpVersion,
				loginId,
				email,
				roleName
			});
		}


		public static async Task<List<Dashboard500VM>> GetDashboard500VMs(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Dashboard500VM>(@"
                select top 10 
                DateTimeUtc, Path, Email 
                from weblog
                where StatusCode = 500
                order by DateTimeUtc desc");

			return result.ToList();
		}

		public static async Task<List<Dashboard404VM>> GetDashboard404VMs(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Dashboard404VM>(@"
                select top 30 
                DateTimeUtc, IPAddress, Path, UserAgent, Email
                from weblog
                where StatusCode = 404
                order by DateTimeUtc desc");

			return result.ToList();
		}

		public static async Task<List<DashboardLoginAndJob>> GetDashboardLoginsAndJobs(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<DashboardLoginAndJob>(@"
                select top 10
                l.Email, j.DateTimeUtcJobStartedOnVM, j.JobTypeId, OrigFileName,
                datediff(second,j.DateTimeUtcJobStartedOnVM,j.DateTimeUtcJobEndedOnVM)  as TimeTakenInS
                from job j
                join login l on l.LoginId = j.LoginId
                order by DateTimeUtcJobStartedOnVM desc");

			return result.ToList();
		}

		public static async Task<List<DashboardRealPage>> GetDashboardRealPages(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<DashboardRealPage>(@"
                select top 20
                DateTimeUtc, IPAddress, Path, UserAgent, Email
                from weblog
                where WebLogTypeId = 1
                order by DateTimeUtc desc");

			return result.ToList();
		}

		public static async Task<List<DashboardRequest>> GetDashboardAllRequests(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<DashboardRequest>(@"
                select top 30
                *
                from weblog
                order by DateTimeUtc desc");

			return result.ToList();
		}

		// HERE
		public static async Task<List<ProjectViewModel>> GetAllPublicChallengeProjects(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectViewModel>(@"
                select ProjectId, Name, DateTimeCreatedUtc, ShortDescription
                from Project 
                where ProjectStatusId = 1
                and IsPublic = 1
                order by DateTimeCreatedUtc desc
                ");

			return result.ToList();
		}

		public static async Task<List<ProjectViewModel>> GetAllPublicOngoingProjects(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectViewModel>(@"
                select ProjectId, Name, DateTimeCreatedUtc, ShortDescription
                from Project 
                where ProjectStatusId = 2
                and IsPublic = 1
                order by DateTimeCreatedUtc desc
                ");

			return result.ToList();
		}

		public static async Task<List<ProjectViewModel>> GetAllPublicCompletedProjects(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectViewModel>(@"
                select ProjectId, Name, DateTimeCreatedUtc, ShortDescription
                from Project 
                where ProjectStatusId = 3
                and IsPublic = 1
                order by DateTimeCreatedUtc desc
                ");

			return result.ToList();
		}

		// used by /issues/  for Admin
		public static async Task<List<IssueViewModel>> GetAllIssues(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<IssueViewModel>(@"
                select i.IssueId, i.Name, i.Description, r.Name as RegulatorName
                from Issue i
				left join Regulator r on r.RegulatorId = i.RegulatorId
                order by DateTimeCreatedUtc desc
                ");

			return result.ToList();
		}

		public static async Task<List<IssueViewModel>> GetAllPublicIssues(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<IssueViewModel>(@"
                select i.IssueId, i.Name, i.Description, r.Name as RegulatorName
                from Issue i
				left join Regulator r on r.RegulatorId = i.RegulatorId
				where IsPublic = 1
                order by DateTimeCreatedUtc desc
                ");

			return result.ToList();
		}

		// used in /projects
		// only Admin - see GetAllPublicProjects below
		// remember to update both!
		public static async Task<List<ProjectFullViewModel>> GetAllProjects(string connectionString, int? statusId = null)
		{
			using var conn = GetOpenConnection(connectionString);

			string sql = @"
                select p.*, l.Email as PromoterEmail
                from Project p
                -- left join so if no promoterLoginId we still get result
                left join login l on p.PromoterLoginId = l.LoginId
            ";
			// challenge, ongoing or completed
			if (statusId == 1 || statusId == 2 || statusId == 3)
			{
				sql += @"
                where p.ProjectStatusId = @StatusId
                order by p.DateTimeCreatedUtc desc
                ";
			}
			else
			{
				// default to display all
				sql += @"
                order by p.DateTimeCreatedUtc desc
                ";
			}

			var result = await conn.QueryAsyncWithRetry<ProjectFullViewModel>(sql, new { statusId });
			return result.ToList();
		}

		// see above for admin query
		// remember to update both!
		public static async Task<List<ProjectFullViewModel>> GetAllPublicProjects(string connectionString, int? statusId = null)
		{
			using var conn = GetOpenConnection(connectionString);

			string sql = @"
                select p.*, l.Email as PromoterEmail
                from Project p
                -- left join so if no promoterLoginId we still get result
                left join login l on p.PromoterLoginId = l.LoginId
            ";
			// challenge, ongoing or completed
			if (statusId == 1 || statusId == 2 || statusId == 3)
			{
				sql += @"
                where p.ProjectStatusId = @StatusId
                and p.IsPublic = 1
                order by p.DateTimeCreatedUtc desc
                ";
			}
			else
			{
				// default to display all
				sql += @"
                where p.IsPublic = 1
                order by p.DateTimeCreatedUtc desc
                ";
			}

			var result = await conn.QueryAsyncWithRetry<ProjectFullViewModel>(sql, new { statusId });
			return result.ToList();
		}

		// /project/2
		public static async Task<ProjectAllTablesViewModel> GetProjectByProjectId(string connectionString, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectAllTablesViewModel>(@"

            select p.*, l.Email as PromoterEmail, ps.Name as ProjectStatusName
            from Project p
            join ProjectStatus ps on p.ProjectStatusId = ps.ProjectStatusId
            -- left join so if no promoterLoginId we still get result
            left join login l on p.PromoterLoginId = l.LoginId

            where p.ProjectId = @ProjectId

            ", new { projectId });

			return result.SingleOrDefault();
		}

		// /project/2
		public static async Task<List<ProjectMembersViewModel>> GetProjectMembersByProjectId(string connectionString, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectMembersViewModel>(@"

            select l.LoginId, l.Email, x.InstitutionName, x.ProfileUrl
            from Login l
            join ProjectLogin pl on l.LoginId = pl.LoginId
			left join Person x on x.LoginId = l.LoginId
            where pl.ProjectId = @ProjectId

            ", new { projectId });

			return result.ToList();
		}

		public static async Task<List<ProjectLinksViewModel>> GetLinksByProjectId(string connectionString, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectLinksViewModel>(@"

            select l.LinkId, l.ProjectId, l.Url, l.Description
            from Link l
            where l.ProjectId = @ProjectId
            ", new { projectId });

			return result.ToList();
		}

		public static async Task<ProjectLinksViewModel> GetLinkByLinkId(string connectionString, int linkId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectLinksViewModel>(@"

            select l.LinkId, l.ProjectId, l.Url, l.Description
            from Link l
            where l.LinkId = @LinkId
            ", new { linkId });

			return result.SingleOrDefault();
		}

		public static async Task<List<ProjectIssueViewModel>> GetIssuesByProjectId(string connectionString, int projectId, bool getPrivateIssues)
		{
			using var conn = GetOpenConnection(connectionString);

			string sql;
			if (getPrivateIssues)
			{
				sql = @"
				select i.IssueId, i.Name, i.Description, r.Name as RegulatorName
				from Issue i
				left join Regulator r on i.RegulatorId = r.RegulatorId
				where ProjectId = @ProjectId
				";
			}
			else
			{
				sql = @"
				select i.IssueId, i.Name, i.Description, r.Name as RegulatorName
				from Issue i
				left join Regulator r on i.RegulatorId = r.RegulatorId
				where i.ProjectId = @ProjectId
				and i.IsPublic = 1
				";
			}

			var result = await conn.QueryAsyncWithRetry<ProjectIssueViewModel>(sql, new { projectId });

			return result.ToList();
		}


		// /project/2
		public static async Task<IssueAllTablesViewModel> GetIssueByIssueId(string connectionString, int issueId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<IssueAllTablesViewModel>(@"

            select i.*, p.Name as ProjectName, r.Name as RegulatorName, ist.Name as IssueStatusName
            from Issue i
            join Project p on p.ProjectId = i.ProjectId 
            join IssueStatus ist on ist.IssueStatusId = i.IssueStatusId
            left join Regulator r on i.RegulatorId = r.RegulatorId
            where i.IssueId = @IssueId 

            ", new { issueId });

			return result.SingleOrDefault();
		}

		// used by /project/2 to see if should display edit button
		public static async Task<bool> CheckIfLoginIdCanSeeEditButtonForProjectId(string connectionString, int loginId, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<bool>(@"
            select count(*) 
            from Project 
            where PromoterLoginId = @PromoterLoginId 
            and ProjectId = @ProjectId
            ", new { promoterLoginId = loginId, projectId });

			// https://stackoverflow.com/a/31282196/26086
			// When  0 is returned Dapper will return False
			return result.FirstOrDefault();

		}

		public static async Task<ProjectEditViewModel> GetProjectEditVMByProjectId(string connectionString, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectEditViewModel>(@"

            select p.*
            from Project p
            where p.ProjectId = @ProjectId

            ", new { projectId });

			return result.SingleOrDefault();
		}

		public static async Task<List<ProjectStatus>> GetAllProjectStatuses(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectStatus>(@"
                select * 
                from ProjectStatus 
                order by ProjectStatusId 
                ");

			return result.ToList();
		}

		public static async Task UpdateProjectByProjectId(string connectionString, int projectId, string name, int projectStatusId,
			bool isPublic, int? promoterLoginId, string shortDescription, string description, string keywords, DateTime dateTimeCreatedUtc,
			string researchNotes)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.ExecuteAsyncWithRetry(@"
                update project
                set Name = @Name,
                    ProjectStatusId = @ProjectStatusId,
                    IsPublic = @IsPublic,
                    PromoterLoginId = @PromoterLoginId,
                    ShortDescription = @ShortDescription,
                    Description = @Description,
                    Keywords = @Keywords,
                    DateTimeCreatedUtc = @DateTimeCreatedUtc,
                    ResearchNotes = @ResearchNotes
                where ProjectId = @ProjectId
                ", new
			{
				projectId,
				name,
				projectStatusId,
				isPublic,
				promoterLoginId,
				shortDescription,
				description,
				keywords,
				dateTimeCreatedUtc,
				researchNotes
			});
		}

		public static async Task<int> CreateProjectAndReturnProjectId(string connectionString, int projectId, string name, int projectStatusId,
		bool isPublic, int? promoterLoginId, string shortDescription, string description, string keywords, DateTime dateTimeCreatedUtc,
		string researchNotes)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
                INSERT INTO Project
           ([Name]
           ,[ProjectStatusId]
           ,[IsPublic]
           ,[PromoterLoginId]
           ,[ShortDescription]
           ,[Description]
           ,[Keywords]
           ,[ResearchNotes])
           output inserted.ProjectId
           VALUES
           (@Name
           ,@ProjectStatusId
           ,@IsPublic
           ,@PromoterLoginId
           ,@ShortDescription
           ,@Description
           ,@Keywords
           ,@ResearchNotes
          )", new
			{
				name,
				projectStatusId,
				isPublic,
				promoterLoginId,
				shortDescription,
				description,
				keywords,
				researchNotes
			});
			return result.Single();
		}

		// promoter is allowed to edit project
		public static async Task<bool> CheckIfLoginIdIsAllowedToEditThisProject(string connectionString, int? loginId, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<bool>(@"
                select count(*) 
                from Project
                where PromoterLoginId = @LoginId
                    and ProjectId = @ProjectId
                ", new { loginId, projectId });

			// https://stackoverflow.com/a/31282196/26086
			// When  0 is returned Dapper will return False
			return result.FirstOrDefault();
		}

		public static async Task<List<PromoterLogin>> GetAllPromoterLogins(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<PromoterLogin>(@"
                select LoginId, Email 
                from Login 
                order by LoginId 
                ");

			return result.ToList();
		}

		public static async Task<List<Regulator>> GetAllRegulators(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<Regulator>(@"
                select * 
                from Regulator 
                order by RegulatorId 
                ");

			return result.ToList();
		}

		public static async Task<IssueEditViewModel> GetIssueEditVMByIssueId(string connectionString, int issueId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<IssueEditViewModel>(@"

            select i.*, p.Name as ProjectName 
            from Issue i
            join Project p on p.ProjectId = i.ProjectId
            where IssueId = @IssueId

            ", new { issueId });

			return result.SingleOrDefault();
		}

		public static async Task<List<IssueStatus>> GetAllIssueStatuses(string connectionString)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<IssueStatus>(@"
                select * 
                from IssueStatus 
                order by IssueStatusId 
                ");

			return result.ToList();
		}

		public static async Task UpdateIssueByIssueId(string connectionString, IssueEditViewModel issue, int? issueStatusId,
			int? regulatorId)
		{
			using var conn = GetOpenConnection(connectionString);
			var i = issue;

			var result = await conn.ExecuteAsyncWithRetry(@"
                update issue
                set Name = @Name,
                    IssueStatusId = @IssueStatusId,
                    IsPublic = @IsPublic,
                    RegulatorId = @RegulatorId,
                    Description = @Description,
                    Keywords = @Keywords,
                    Response = @Response
                where IssueId = @IssueId
                ", new
			{
				i.IssueId,
				i.Name,
				issueStatusId,
				i.IsPublic,
				regulatorId,
				i.Description,
				i.Keywords,
				i.Response
			});
		}

		public static async Task<int> CreateIssueAndReturnIssueId(string connectionString, IssueEditViewModel issue, int? issueStatusId,
			int? regulatorId, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
               INSERT INTO [dbo].[Issue]
           ([ProjectId]
           ,[RegulatorId]
           ,[Name]
           ,[IssueStatusId]
           ,[IsPublic]
           ,[Description]
           ,[Keywords]
           ,[Response]
			)
           output inserted.IssueId
           VALUES
           (@ProjectId
           ,@RegulatorId
           ,@Name
           ,@IssueStatusId
           ,@IsPublic
           ,@Description
           ,@Keywords
           ,@Response
          )", new
			{
				projectId,
				regulatorId,
				name = issue.Name,
				issueStatusId,
				isPublic = issue.IsPublic,
				description = issue.Description,
				keywords = issue.Keywords,
				response = issue.Response
			});
			return result.Single();
		}

		public static async Task<int> CreateLinkAndReturnLinkId(string connectionString, ProjectLinksViewModel link, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<int>(@"
              INSERT INTO [dbo].[Link]
           ([ProjectId]
           ,[Url]
           ,[Description])
           output inserted.LinkId
           VALUES
           (@ProjectId
			,@Url
            ,@Description
			)", new
			{
				projectId,
				url = link.Url,
				description = link.Description,
			});
			return result.Single();
		}

		public static async Task DeleteLinkByLinkId(string connectionString, int? linkId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.ExecuteAsyncWithRetry(@"
				delete from link
				where linkId = @LinkId
                ", new { linkId });
		}

		public static async Task<List<LoginMemberVM>> GetLoginsNotInProject(string connectionString, int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			// https://stackoverflow.com/a/5676632/26086
			var result = await conn.QueryAsyncWithRetry<LoginMemberVM>(@"
                select l.LoginId, l.Email
                from Login l
				left join ProjectLogin pl on pl.LoginId = l.LoginId
				where pl.ProjectLoginId IS NULL
                ");

			return result.ToList();
		}

		public static async Task AddLoginIdToProjectLogin(string connectionString, int selectedLoginId,
			int projectId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.ExecuteAsyncWithRetry(@"
				insert into ProjectLogin
				(ProjectId, LoginId)
				values
				(@ProjectId, @LoginId)
                ", new { projectId, loginId = selectedLoginId });
		}

		public static async Task<ProjectMembersViewModel> GetLoginByProjectIdAndLoginId(
			string connectionString, int projectId, int? loginId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<ProjectMembersViewModel>(@"

            select l.LoginId, l.Email
            from Login l 
            join ProjectLogin pl on pl.LoginId = l.LoginId
            where pl.ProjectId = @ProjectId 
			and l.LoginId = @LoginId

            ", new { projectId, loginId });

			return result.SingleOrDefault();
		}

		public static async Task DeleteProjectLoginByProjectIdAndLoginId(string connectionString,
			int projectId, int? loginId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.ExecuteAsyncWithRetry(@"
				delete from ProjectLogin
				where projectId = @ProjectId
				and loginId = @LoginId
                ", new { projectId, loginId });
		}

		public static async Task<LogindAndPerson> GetLoginAndPersonByLoginId(string connectionString,
			int? loginId)
		{
			using var conn = GetOpenConnection(connectionString);

			var result = await conn.QueryAsyncWithRetry<LogindAndPerson>(@"

            select l.LoginId, l.Email, x.ContactNumber, x.InstitutionName, x.ProfileUrl, x.Name
            from Login l
			left join Person x on x.LoginId = l.LoginId
            where l.LoginId = @LoginId 
            ", new { loginId });

			// will error if more than one element
			return result.SingleOrDefault();
		}

		public static async Task UpdateLoginAndPersonByLoginId(string connectionString, LogindAndPerson loginAndPerson)
		{
			using var conn = GetOpenConnection(connectionString);

			// does the Person row exist yet for this login?
			// upsert concept 
			// https://stackoverflow.com/questions/108403/solutions-for-insert-or-update-on-sql-server
			// keep it simple here

			var foo = await conn.QueryAsyncWithRetry<int>(@"
				select count(*) from Person where loginId = @LoginId
			", new { loginAndPerson.LoginId });

			bool personIsThere = foo.FirstOrDefault() != 0;
			if (personIsThere)
			{
				// do an update
				var result = await conn.ExecuteAsyncWithRetry(@"
				update Person
				set ContactNumber = @ContactNumber,
					InstitutionName = @InstitutionName,
					ProfileUrl = @ProfileUrl,
					Name = @Name
				where LoginId = @LoginId
                ", new
				{
					loginAndPerson.LoginId,
					loginAndPerson.ContactNumber,
					loginAndPerson.InstitutionName,
					loginAndPerson.ProfileUrl,
					loginAndPerson.Name
				});
			}
			else
			{
				// do an insert
				var result = await conn.ExecuteAsyncWithRetry(@"
				insert into Person
				(LoginId, ContactNumber, InstitutionName, ProfileUrl, Name)
				values
				(@LoginId, @ContactNumber, @InstitutionName, @ProfileUrl, @Name)
                ", new
				{
					loginAndPerson.LoginId,										
					loginAndPerson.ContactNumber,
					loginAndPerson.InstitutionName,
					loginAndPerson.ProfileUrl,
					loginAndPerson.Name
				});
			}
		}

		//**HRE put in Peroject
		//GetProjectByProjectId


		public static class WebLogTypeId
		{
			public const int Page = 1;
			public const int Asset = 2;
			public const int HealthCheckPage = 3;
			public const int RobotsTxt = 4;
			public const int SitemapXml = 5;
			public const int FaviconIco = 6;
			public const int TusFiles = 7;
			public const int Downloads = 8;
		}

		//public static async Task InsertUpdateCookie(string connectionString,
		//    string? cookieValue,
		//    string? loginId,
		//    DateTimeOffset? issuedUtc,
		//    DateTimeOffset? expiresUtc)
		//{
		//    using var conn = GetOpenConnection(connectionString);

		//    await conn.ExecuteAsyncWithRetry(@"
		//     -- The same cookie is in the db already
		//     IF EXISTS (SELECT 1 FROM Cookie WHERE LoginId = @LoginId AND expiresUtc = @ExpiresUtc)
		//     BEGIN
		//        -- do nothing
		//        SELECT 1
		//     END
		//     ELSE
		//     BEGIN
		//         -- There may be an expired cookie so delete
		//         DELETE FROM Cookie WHERE LoginId = @LoginID

		//         INSERT INTO Cookie
		//            (CookieValue,LoginId,IssuedUtc,ExpiresUtc)
		//         VALUES (@CookieValue, @LoginId, @IssuedUtc, @ExpiresUtc)
		//     END

		//    ", new
		//    {
		//        cookieValue,
		//        loginId,
		//        issuedUtc,
		//        expiresUtc
		//    });

		//}

	}
}
