#ifndef LOCALSTORAGESERVICE_H
#define LOCALSTORAGESERVICE_H

#include <QObject>
#include <QSqlDatabase>
#include "../Models/onlinevideomodel.h"
#include "../Models/releasemodel.h"
#include "../Models/releasetorrentmodel.h"

class LocalStorageService : public QObject
{
    Q_OBJECT

private:
    QSqlDatabase m_Database;

    QString videosToJson(QList<OnlineVideoModel>& videos);
    QString torrentsToJson(QList<ReleaseTorrentModel>& torrents);
    bool isReleaseExists(int id);
    void insertRelease(ReleaseModel& releaseModel);
    void updateRelease(ReleaseModel& releaseModel);
public:
    explicit LocalStorageService(QObject *parent = nullptr);
    ~LocalStorageService();

    Q_INVOKABLE void updateAllReleases(const QString& releases);
    Q_INVOKABLE void addOrUpdateRelease(const QString& release);
    Q_INVOKABLE QString getRelease(int id);
    Q_INVOKABLE QString getReleasesByFilter();
    Q_INVOKABLE void setSchedule(QString schedule);
signals:

public slots:


};

#endif // LOCALSTORAGESERVICE_H
