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
    bool IsReleaseExists(int id);

public:
    explicit LocalStorageService(QObject *parent = nullptr);
    ~LocalStorageService();

    Q_INVOKABLE void AddOrUpdateRelease(const QString& release);
    Q_INVOKABLE void UpdateRelease(const ReleaseModel& release);
    Q_INVOKABLE QString GetRelease(int id);
    Q_INVOKABLE QStringList GetReleasesPage(int page);
    Q_INVOKABLE void SetSchedule(QString schedule);
signals:

public slots:


};

#endif // LOCALSTORAGESERVICE_H
