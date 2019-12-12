#include "localstorageservice.h"
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QStandardPaths>
#include <QVariant>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonValue>
#include "../Models/releasemodel.h"

LocalStorageService::LocalStorageService(QObject *parent) : QObject(parent)
{
    m_Database = QSqlDatabase::addDatabase("QSQLITE");
    auto path = QStandardPaths::writableLocation(QStandardPaths::AppLocalDataLocation) + "/cache.db";
    m_Database.setDatabaseName(path);
    m_Database.open();

    QSqlQuery query(m_Database);

    QString releasesTable = "CREATE TABLE IF NOT EXISTS `Releases` (";
    releasesTable += "`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,";
    releasesTable += "`Title` TEXT NOT NULL,";
    releasesTable += "`Code` TEXT NOT NULL,";
    releasesTable += "`OriginalTitle` TEXT NOT NULL,";
    releasesTable += "`ReleaseId` INTEGER NOT NULL,";
    releasesTable += "`Rating` BIGINT NOT NULL,";
    releasesTable += "`Series` TEXT,";
    releasesTable += "`Status` TEXT NOT NULL,";
    releasesTable += "`Type` TEXT NOT NULL,";
    releasesTable += "`Timestamp` INTEGER NOT NULL,";
    releasesTable += "`Year` TEXT NOT NULL,";
    releasesTable += "`Season` TEXT NOT NULL,";
    releasesTable += "`CountOnlineVideos` INTEGER NOT NULL,";
    releasesTable += "`TorrentsCount` INTEGER NOT NULL,";
    releasesTable += "`Description` TEXT NOT NULL,";
    releasesTable += "`Announce` TEXT,";
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

void LocalStorageService::updateAllReleases(const QString &releases)
{
    QJsonParseError jsonError;
    QJsonDocument jsonDocument = QJsonDocument::fromJson(releases.toUtf8(), &jsonError);

    if (m_Database.transaction()) return;

    QJsonArray jsonReleases = jsonDocument.array();
    foreach (QJsonValue jsonRelease,  jsonReleases) {
        ReleaseModel releaseModel;
        releaseModel.readFromApiModel(jsonRelease.toObject());

        if (isReleaseExists(releaseModel.id())) {
            updateRelease(releaseModel);
            return;
        }

        insertRelease(releaseModel);
    }

    if (!m_Database.commit()) m_Database.rollback();
}

QString LocalStorageService::videosToJson(QList<OnlineVideoModel> &videos)
{
    QJsonArray videosArray;
    foreach (auto video, videos) {
        QJsonObject jsonObject;
        video.writeToJson(jsonObject);
        videosArray.append(jsonObject);
    }
    QJsonDocument videoDocument(videosArray);
    QString videosJson(videoDocument.toJson());
    return videosJson;
}

QString LocalStorageService::torrentsToJson(QList<ReleaseTorrentModel> &torrents)
{
    QJsonArray torrentsArray;
    foreach (auto torrent, torrents) {
        QJsonObject jsonObject;
        torrent.writeToJson(jsonObject);
        torrentsArray.append(jsonObject);
    }
    QJsonDocument torrentDocument(torrentsArray);
    QString torrentJson(torrentDocument.toJson());
    return torrentJson;
}

bool LocalStorageService::isReleaseExists(int id)
{
    QSqlQuery query(m_Database);
    query.prepare("SELECT `ReleaseId` FROM `Releases` WHERE `ReleaseId` = :id");

    query.bindValue(":id", id);

    query.exec();

    return query.next();
}

void LocalStorageService::insertRelease(ReleaseModel &releaseModel)
{
    auto torrents = releaseModel.torrents();
    auto torrentJson = torrentsToJson(torrents);

    auto videos = releaseModel.videos();
    auto videosJson = videosToJson(videos);

    QSqlQuery query(m_Database);
    QString request = "INSERT INTO `Releases` (`Title`,`Code`,`OriginalTitle`,`ReleaseId`,`Rating`,`Series`,`Status`,`Type`,`Timestamp`,";
    request.append("`Year`,`Season`,`CountOnlineVideos`,`TorrentsCount`,`Description`,`Announce`,`Genres`,`Poster`,`Voices`,`Torrents`,`Videos`,`ScheduleOnDay`, `MetaData`) ");
    auto values = QString(" VALUES (:title,:code,:originaltitle,:id,:rating,:series,:status,:type,:timestamp,:year,:season,:videoscount,:torrentscount,:description,:announce,:genres,:poster,:voices,:torrents,:videos,:scheduleonday,:metadata)");

    query.prepare(request + values);

    auto voices = releaseModel.voices().join(",");
    if (voices.length() == 0) voices = "Не указано";

    auto genres = releaseModel.genres().join(",");
    if (genres.length() == 0) genres = "Не указано";

    query.bindValue(":title", releaseModel.title());
    query.bindValue(":code", releaseModel.code());
    query.bindValue(":originaltitle", releaseModel.names().last());
    query.bindValue(":id", releaseModel.id());
    query.bindValue(":rating", releaseModel.rating());
    query.bindValue(":series", releaseModel.series());
    query.bindValue(":status", releaseModel.status());
    query.bindValue(":type", releaseModel.type());
    query.bindValue(":timestamp", releaseModel.timestamp());
    query.bindValue(":year", releaseModel.year());
    query.bindValue(":season", releaseModel.season());
    query.bindValue(":videoscount", releaseModel.videos().length());
    query.bindValue(":torrentscount", releaseModel.torrents().length());
    query.bindValue(":description", releaseModel.description());
    query.bindValue(":announce", releaseModel.announce());
    query.bindValue(":genres", genres);
    query.bindValue(":poster", releaseModel.poster());
    query.bindValue(":voices", voices);
    query.bindValue(":torrents", torrentJson);
    query.bindValue(":videos", videosJson);
    query.bindValue(":scheduleonday", "понедельник");
    query.bindValue(":metadata", "{}");

    query.exec();
}

void LocalStorageService::addOrUpdateRelease(const QString &release)
{
    ReleaseModel releaseModel;
    QJsonParseError jsonError;

    QJsonDocument doc = QJsonDocument::fromJson(release.toUtf8(), &jsonError);
    if (jsonError.error != QJsonParseError::NoError) {
        //TODO: need handle this situation
    }

    releaseModel.readFromApiModel(doc.object());

    if (isReleaseExists(releaseModel.id())) {
        updateRelease(releaseModel);
        return;
    }

    auto torrents = releaseModel.torrents();
    auto torrentJson = torrentsToJson(torrents);

    auto videos = releaseModel.videos();
    auto videosJson = videosToJson(videos);

    QSqlQuery query(m_Database);
    QString request = "INSERT INTO `Releases` (`Title`,`Code`,`OriginalTitle`,`ReleaseId`,`Rating`,`Series`,`Status`,`Type`,`Timestamp`,";
    request.append("`Year`,`Season`,`CountOnlineVideos`,`TorrentsCount`,`Description`,`Announce`,`Genres`,`Poster`,`Voices`,`Torrents`,`Videos`,`ScheduleOnDay`, `MetaData`) ");
    auto values = QString(" VALUES (:title,:code,:originaltitle,:id,:rating,:series,:status,:type,:timestamp,:year,:season,:videoscount,:torrentscount,:description,:announce,:genres,:poster,:voices,:torrents,:videos,:scheduleonday,:metadata)");

    query.prepare(request + values);

    auto voices = releaseModel.voices().join(",");
    if (voices.length() == 0) voices = "Не указано";

    auto genres = releaseModel.genres().join(",");
    if (genres.length() == 0) genres = "Не указано";

    query.bindValue(":title", releaseModel.title());
    query.bindValue(":code", releaseModel.code());
    query.bindValue(":originaltitle", releaseModel.names().last());
    query.bindValue(":id", releaseModel.id());
    query.bindValue(":rating", releaseModel.rating());
    query.bindValue(":series", releaseModel.series());
    query.bindValue(":status", releaseModel.status());
    query.bindValue(":type", releaseModel.type());
    query.bindValue(":timestamp", releaseModel.timestamp());
    query.bindValue(":year", releaseModel.year());
    query.bindValue(":season", releaseModel.season());
    query.bindValue(":videoscount", releaseModel.videos().length());
    query.bindValue(":torrentscount", releaseModel.torrents().length());
    query.bindValue(":description", releaseModel.description());
    query.bindValue(":announce", releaseModel.announce());
    query.bindValue(":genres", genres);
    query.bindValue(":poster", releaseModel.poster());
    query.bindValue(":voices", voices);
    query.bindValue(":torrents", torrentJson);
    query.bindValue(":videos", videosJson);
    query.bindValue(":scheduleonday", "понедельник");
    query.bindValue(":metadata", "{}");

    query.exec();
    /*if (!query.exec()) {
        auto error = query.lastError().text();
        if (error.length() > 0) {

        }
    }*/

}

void LocalStorageService::updateRelease(ReleaseModel& releaseModel)
{
    auto torrents = releaseModel.torrents();
    auto torrentJson = torrentsToJson(torrents);

    auto videos = releaseModel.videos();
    auto videosJson = videosToJson(videos);

    QSqlQuery query(m_Database);
    QString request = "UPDATE `Releases` SET `Title` = :title,`Code` = :code,`OriginalTitle` = :originaltitle,`Rating` = :rating,`Series` = :series,`Status` = :status,`Type` = :type,`Timestamp` = :timestamp,";
    request.append("`Year` = :year,`Season` = :season,`CountOnlineVideos` = :videoscount,`TorrentsCount` = :torrentscount,`Description` = :description,`Announce` = :announce,`Genres` = :genres,`Poster` = :poster,`Voices` = :voices,`Torrents = :torrents,`Videos` = :videos,`ScheduleOnDay` = :scheduleonday, `MetaData` = :metadata) ");
    auto values = QString(" VALUES (:title,:code,:originaltitle,:rating,:series,:status,:type,:timestamp,:year,:season,:videoscount,:torrentscount,:description,:announce,:genres,:poster,:voices,:torrents,:videos,:scheduleonday,:metadata)");

    query.prepare(request + values);

    auto voices = releaseModel.voices().join(",");
    if (voices.length() == 0) voices = "Не указано";

    auto genres = releaseModel.genres().join(",");
    if (genres.length() == 0) genres = "Не указано";

    query.bindValue(":title", releaseModel.title());
    query.bindValue(":code", releaseModel.code());
    query.bindValue(":originaltitle", releaseModel.names().last());
    query.bindValue(":id", releaseModel.id());
    query.bindValue(":rating", releaseModel.rating());
    query.bindValue(":series", releaseModel.series());
    query.bindValue(":status", releaseModel.status());
    query.bindValue(":type", releaseModel.type());
    query.bindValue(":timestamp", releaseModel.timestamp());
    query.bindValue(":year", releaseModel.year());
    query.bindValue(":season", releaseModel.season());
    query.bindValue(":videoscount", releaseModel.videos().length());
    query.bindValue(":torrentscount", releaseModel.torrents().length());
    query.bindValue(":description", releaseModel.description());
    query.bindValue(":announce", releaseModel.announce());
    query.bindValue(":genres", genres);
    query.bindValue(":poster", releaseModel.poster());
    query.bindValue(":voices", voices);
    query.bindValue(":torrents", torrentJson);
    query.bindValue(":videos", videosJson);
    query.bindValue(":scheduleonday", "понедельник");
    query.bindValue(":metadata", "{}");

    query.exec();
}

QString LocalStorageService::getRelease(int id)
{
    if (id > 0) {

    }
    return "";
}

QStringList LocalStorageService::getReleasesPage(int page)
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
        if (id > 0) {

        }

        stringList.append(resultJson);
    }

    return stringList;
}

void LocalStorageService::setSchedule(QString schedule)
{
    if (schedule.length() > 0) {

    }
}
