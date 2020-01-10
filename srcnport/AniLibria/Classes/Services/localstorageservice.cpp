#include "localstorageservice.h"
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QStandardPaths>
#include <QVariant>
#include <QJsonDocument>
#include <QJsonObject>
#include <QJsonValue>
#include <QtConcurrent>
#include <QFuture>
#include <QDebug>
#include <QFutureWatcher>
#include "../Models/releasemodel.h"
#include "../Models/fullreleasemodel.h"

LocalStorageService::LocalStorageService(QObject *parent) : QObject(parent)
{
    m_Database = QSqlDatabase::addDatabase("QSQLITE");
    m_AllReleaseUpdatedWatcher = new QFutureWatcher<void>(this);
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

    query.exec(releasesTable);
    query.exec("CREATE TABLE IF NOT EXISTS `Schedule` (`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `Metadata` TEXT NOT NULL)");
    query.exec("CREATE TABLE IF NOT EXISTS `Favorites` (`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `Metadata` TEXT NOT NULL)");

    connect(m_AllReleaseUpdatedWatcher, SIGNAL(finished()), this, SLOT(allReleasesUpdated()));
}

LocalStorageService::~LocalStorageService()
{
    m_Database.close();
}

void LocalStorageService::updateAllReleases(const QString &releases)
{
    QFuture<void> future = QtConcurrent::run(
        [=] {
            QJsonParseError jsonError;
            QJsonDocument jsonDocument = QJsonDocument::fromJson(releases.toUtf8(), &jsonError);

            if (!m_Database.transaction()) return;

            QJsonArray jsonReleases = jsonDocument.array();
            foreach (QJsonValue jsonRelease,  jsonReleases) {
                ReleaseModel releaseModel;
                releaseModel.readFromApiModel(jsonRelease.toObject());

                if (isReleaseExists(releaseModel.id())) {
                    updateRelease(releaseModel);
                    continue;
                }

                insertRelease(releaseModel);
            }

            if (!m_Database.commit()) m_Database.rollback();
        }
    );
    m_AllReleaseUpdatedWatcher->setFuture(future);
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

    auto voices = releaseModel.voices().join(", ");
    if (voices.length() == 0) voices = "Не указано";

    auto genres = releaseModel.genres().join(", ");
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

void LocalStorageService::updateRelease(ReleaseModel& releaseModel)
{
    auto torrents = releaseModel.torrents();
    auto torrentJson = torrentsToJson(torrents);

    auto videos = releaseModel.videos();
    auto videosJson = videosToJson(videos);

    auto voices = releaseModel.voices().join(", ");
    if (voices.length() == 0) voices = "Не указано";

    auto genres = releaseModel.genres().join(", ");
    if (genres.length() == 0) genres = "Не указано";

    QSqlQuery query(m_Database);
    QString request = "UPDATE `Releases` SET `Title` = ?,`Code` = ?,`OriginalTitle` = ?,`Rating` = ?,`Series` = ?,`Status` = ?,`Type` = ?,`Timestamp` = ?, ";
    request.append("`Year` = ?, `Season` = ?, `CountOnlineVideos` = ?, `TorrentsCount` = ?, `Description` = ?, `Announce` = ?, `Genres` = ?, `Poster` = ?, `Voices` = ? ");
    request.append(" WHERE `ReleaseId` = ?");

    query.prepare(request);

    query.bindValue(0, releaseModel.title());
    query.bindValue(1, releaseModel.code());
    query.bindValue(2, releaseModel.names().last());
    query.bindValue(3, releaseModel.rating());
    query.bindValue(4, releaseModel.series());
    query.bindValue(5, releaseModel.status());
    query.bindValue(6, releaseModel.type());
    query.bindValue(7, releaseModel.timestamp());
    query.bindValue(8, releaseModel.year());
    query.bindValue(9, releaseModel.season());
    query.bindValue(10, releaseModel.videos().length());
    query.bindValue(11, releaseModel.torrents().length());
    query.bindValue(12, releaseModel.description());
    query.bindValue(13, releaseModel.announce());
    query.bindValue(14, genres);
    query.bindValue(15, releaseModel.poster());
    query.bindValue(16, voices);
    query.bindValue(17, releaseModel.id());

    if (!query.exec()) {
        const QString errorLine = query.lastError().text();
        qDebug() << errorLine;
    }

    QSqlQuery jsonQuery(m_Database);
    QString jsonQueryString = "UPDATE `Releases` SET `Torrents` = ?, `Videos` = ? WHERE `ReleaseId` = " + QString::number(releaseModel.id());

    jsonQuery.prepare(jsonQueryString);

    jsonQuery.bindValue(0, torrentJson);
    jsonQuery.bindValue(1, videosJson);

    if (!jsonQuery.exec()) {
        const QString errorLine = jsonQuery.lastError().text();
        qDebug() << errorLine;
    }
}

QStringList LocalStorageService::getAllFavorites()
{
    QSqlQuery query(m_Database);

    query.exec("SELECT * FROM `Favorites`");

    if (!query.next()) return QStringList();

    auto metadata = query.value("Metadata").toString();
    QJsonParseError jsonError;
    QJsonDocument jsonDocument = QJsonDocument::fromJson(metadata.toUtf8(), &jsonError);
    auto favorites = jsonDocument.array();
    QStringList result;
    foreach (auto favorite, favorites) result.append(QString::number(favorite.toInt()));

    return result;
}

QString LocalStorageService::getRelease(int id)
{
    QSqlQuery query(m_Database);

    query.prepare("SELECT * FROM `Releases` WHERE `ReleaseId` = ?");

    query.bindValue(0, id);

    if (!query.exec()) return "null";

    FullReleaseModel release;
    release.fromDatabase(query);
    QJsonObject jsonValue;
    release.writeToJson(jsonValue);

    QJsonDocument saveDoc(jsonValue);
    return saveDoc.toJson();
}

QString LocalStorageService::getReleasesByFilter(int page, QString title, int section)
{
    QSqlQuery query(m_Database);
    int pageSize = 12;
    int startIndex = (page - 1) * pageSize;

    QString request = "SELECT `Id`, `Title`,`Code`,`OriginalTitle`,`ReleaseId`,`Rating`,`Series`,`Status`,`Type`,`Timestamp`,";
    request += "`Year`,`Season`,`CountOnlineVideos`,`TorrentsCount`,`Description`,`Announce`,`Genres`,`Poster`,`Voices`,`Torrents`,`Videos`,`ScheduleOnDay` FROM `Releases` ";
    request += " ORDER BY `Timestamp` DESC";
    query.exec(request);

    QJsonArray releases;

    QStringList userFavorites = getAllFavorites();

    //WORKAROUND: because unicode `LIKE` and `ORDER` don't work correctly I did all in c++
    int index = -1;
    while (query.next())
    {
        if (!title.isEmpty() && !query.value("Title").toString().toLower().contains(title.toLower())) continue;

        if (section == 1) {
            auto releaseId = query.value("Releaseid").toInt();
            if (!userFavorites.contains(QString::number(releaseId))) continue;
        }

        index++;
        if (index < startIndex) continue;
        if (releases.count() == pageSize) continue;

        FullReleaseModel release;
        release.fromDatabase(query);
        QJsonObject jsonValue;
        release.writeToJson(jsonValue);
        releases.append(jsonValue);
    }

    QJsonDocument saveDoc(releases);
    return saveDoc.toJson();
}

void LocalStorageService::setSchedule(QString schedule)
{
    QSqlQuery query(m_Database);
    query.exec("SELECT * FROM `Schedule` LIMIT 1");
    if (!query.next()) {
        query.exec("INSERT INTO `Schedule`(`Metadata`) VALUES ('-')");
        query.exec("SELECT * FROM `Schedule` LIMIT 1");
        query.next();
    }
    auto id = query.value("Id").toInt();

    query.prepare("UPDATE `Schedule` SET `Metadata` = ? WHERE `Id` = ?");
    query.bindValue(0, schedule);
    query.bindValue(1, id);
    query.exec();
}

QString LocalStorageService::getSchedule()
{
    QSqlQuery query(m_Database);
    query.exec("SELECT * FROM `Schedule` LIMIT 1");
    if (!query.next()) return "{}";

    return query.value("Metadata").toString();
}

void LocalStorageService::updateFavorites(QString data)
{
    QSqlQuery query(m_Database);

    query.exec("SELECT * FROM `Favorites`");

    if (!query.next()) {
        QSqlQuery query(m_Database);
        query.prepare("INSERT INTO `Favorites`(`Metadata`) VALUES (?)");
        query.bindValue(0, data);
        query.exec();
    } else {
        QSqlQuery query(m_Database);
        query.prepare("UPDATE `Favorites` SET `Metadata`= ?");
        query.bindValue(0, data);
        query.exec();
    }
}

QList<int> LocalStorageService::getFavorites()
{
    auto favorites = getAllFavorites();
    QList<int> ids;
    foreach(auto favorite, favorites) ids.append(favorite.toInt());

    return ids;
}

void LocalStorageService::allReleasesUpdated()
{
    emit allReleasesFinished();
}
