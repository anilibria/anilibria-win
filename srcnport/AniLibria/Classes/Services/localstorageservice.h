#ifndef LOCALSTORAGESERVICE_H
#define LOCALSTORAGESERVICE_H

#include <QObject>
#include "../Models/onlinevideomodel.h"
#include "../Models/releasemodel.h"
#include "../Models/releasetorrentmodel.h"
#include "../Models/fullreleasemodel.h"
#include "../Models/changesmodel.h"

class LocalStorageService : public QObject
{
    Q_OBJECT

private:
    QFutureWatcher<void>* m_AllReleaseUpdatedWatcher;
    QList<FullReleaseModel>* m_CachedReleases;
    ChangesModel* m_ChangesModel;

    QString videosToJson(QList<OnlineVideoModel>& videos);
    QString torrentsToJson(QList<ReleaseTorrentModel>& torrents);
    FullReleaseModel getReleaseFromCache(int id);
    FullReleaseModel mapToFullReleaseModel(ReleaseModel& releaseModel);
    void saveCachedReleasesToFile();
    QStringList getAllFavorites();
    QMap<int, int> getScheduleAsMap();
    bool checkOrCondition(QStringList source, QStringList target);
    bool checkAllCondition(QStringList source, QStringList target);
    void removeTrimsInStringCollection(QStringList& list);
    int randomBetween(int low, int high, uint seed);
    QString getReleasesCachePath() const;
    QString getFavoritesCachePath() const;
    QString getScheduleCachePath() const;
    QString getSeensCachePath() const;
    QString getNotificationCachePath() const;
    void createIfNotExistsFile(QString path, QString defaultContent);
    void saveChanges();

public:
    explicit LocalStorageService(QObject *parent = nullptr);

    Q_INVOKABLE void updateAllReleases(const QString& releases);
    Q_INVOKABLE QString getRelease(int id);
    Q_INVOKABLE QString getRandomRelease();
    Q_INVOKABLE QString getChanges();    
    Q_INVOKABLE QString getReleasesByFilter(int page, QString title, int section, QString description, QString type, QString genres, bool genresOr, QString voices, bool voicesOr, QString years, QString seasones, QString statuses, int sortingField, bool soringDescending);
    Q_INVOKABLE void setSchedule(QString schedule);
    Q_INVOKABLE QString getSchedule();
    Q_INVOKABLE void updateFavorites(QString data);
    Q_INVOKABLE QList<int> getFavorites();
    Q_INVOKABLE void clearFavorites();
    Q_INVOKABLE void updateReleasesInnerCache();
    Q_INVOKABLE QList<int> getChangesCounts();
    Q_INVOKABLE bool isChangesExists();

signals:
    void allReleasesFinished();

public slots:
    void allReleasesUpdated();

};

#endif // LOCALSTORAGESERVICE_H
