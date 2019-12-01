#include "localstorageservice.h"
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QStandardPaths>
#include <QVariant>
#include <QJsonDocument>
#include "../Models/releasemodel.h"

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
    releasesTable += "`Timestamp` INTEGER NOT NULL,";
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

void LocalStorageService::AddRelease(const QString &release)
{
    ReleaseModel releaseModel;
    QJsonParseError jsonError;

    QJsonDocument doc = QJsonDocument::fromJson(release.toUtf8(), &jsonError);
    if (jsonError.error != QJsonParseError::NoError) //TODO: need handle this situation

    releaseModel.readFromJson(doc.object());

    QSqlQuery query;
    QString request = "INSERT INTO `Releases` (`Title`,`Code`,`OriginalTitle`,`ReleaseId`,`Rating`,`Series`,`Status`,`Type`,`Timestamp`,";
    request += "`Year`,`Season`,`CountOnlineVideos`,`TorrentsCount`,`Description`,`Announce`,`Genres`,`Poster`,`Voices`,`Torrents`,`Videos`,`ScheduleOnDay`) ";
    request += QString(" VALUES ('%1','%2', '%3') ").arg(
        releaseModel.title(),
        releaseModel.code(),
        releaseModel.names().end(),
        releaseModel.id(),
        releaseModel.rating(),
        releaseModel.series(),
        releaseModel.status(),
        releaseModel.type(),
        releaseModel.timestamp(),
        releaseModel.year(),
        releaseModel.season(),
        releaseModel.videos().length(),
        releaseModel.torrents().length(),
        releaseModel.description(),
        releaseModel.announce(),
        releaseModel.genres().join(","),
        releaseModel.poster(),
        releaseModel.voices().join(","),

    );
    query.exec(request);
}

void LocalStorageService::UpdateRelease(const QString& release)
{
    if (release.length() > 1) {

    }
}

QString LocalStorageService::GetRelease(int id)
{
    if (id > 0) {

    }
    return "";
}

QStringList LocalStorageService::GetReleasesPage(int page)
{
    QSqlQuery query;
    auto skip = 10 * (page - 1);
    QString limitClause = "";
    limitClause.append(skip);
    limitClause.append(",");
    limitClause.append(page);

    QString request = "SELECT `Id`, `Title`,`Code`,`OriginalTitle`,`ReleaseId`,`Rating`,`Series`,`Status`,`Type`,`Timestamp`,";
    request += "`Year`,`Season`,`CountOnlineVideos`,`TorrentsCount`,`Description`,`Announce`,`Genres`,`Poster`,`Voices`,`Torrents`,`Videos`,`ScheduleOnDay` ";
    request += "FROM `Releases` ORDER BY `Timestamp` ASC LIMIT " + limitClause;
    query.exec(request);

    QStringList stringList;
    while (query.next())
    {
        QString resultJson;
        int id = query.value(0).toInt();

        stringList.append(resultJson);
    }

    return stringList;
}
