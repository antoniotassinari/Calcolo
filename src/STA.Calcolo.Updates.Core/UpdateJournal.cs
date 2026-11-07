namespace STA.Calcolo.Updates;
public sealed record UpdateJournal(string Root, string Backup, bool ServiceWasRunning, List<FileChange> Changes);
