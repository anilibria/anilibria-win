#include "localstorageservice.h"
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QStandardPaths>

LocalStorageService::LocalStorageService(QObject *parent) : QObject(parent)
{
    m_Database = QSqlDatabase::addDatabase("QSQLITE");
    auto path = QStandardPaths::writableLocation(QStandardPaths::AppLocalDataLocation) + "/cache.db";
    m_Database.setDatabaseName(path);
    m_Database.open();

    QSqlQuery query;

    QString releasesTable = "CREATE TABLE IF NOT EXISTS `Releases` (";
    releasesTable += "`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,";
    releasesTable += "`Title` TEXT NOT NULL,";
    releasesTable += "`Code` TEXT NOT NULL,";
    releasesTable += "`OriginalTitle` TEXT NOT NULL,";
    releasesTable += "`ReleaseId` INTEGER NOT NULL,";
    releasesTable += "`Rating` BIGINT NOT NULL,";
    releasesTable += "`Series` TEXT NOT NULL,";
    releasesTable += "`Status` TEXT NOT NULL,";
    releasesTable += "`Type` TEXT NOT NULL,";
    releasesTable += "`Year` TEXT NOT NULL,";
    releasesTable += "`Season` TEXT NOT NULL,";
    releasesTable += "`CountOnlineVideos` INTEGER NOT NULL,";
    releasesTable += "`TorrentsCount` INTEGER NOT NULL,";
    releasesTable += "`Description` TEXT NOT NULL,";
    releasesTable += "`Announce` TEXT NOT NULL,";
    releasesTable += "`Genres` TEXT NOT NULL,";
    releasesTable += "`Poster` TEXT NOT NULL,";
    releasesTable += "`Voices` TEXT NOT NULL,";
    releasesTable += "`Torrents` TEXT NOT NULL,";
    releasesTable += "`Videos` TEXT NOT NULL,";
    releasesTable += "`ScheduleOnDay` TEXT NOT NULL,";
    releasesTable += "`MetaData` TEXT NOT NULL";
    releasesTable += ")";

    query.prepare(releasesTable);
    query.exec();
}

LocalStorageService::~LocalStorageService()
{
    m_Database.close();
}
