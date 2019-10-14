#ifndef RELEASESSERVICE_H
#define RELEASESSERVICE_H

#include <QObject>
#include <QQmlListProperty>
#include "../Models/releaseitemmodel.h"
#include "releaseloader.h"

class ReleasesService : public QObject
{
    Q_OBJECT
    Q_PROPERTY(QQmlListProperty<ReleaseItemModel> releases READ releases NOTIFY releasesChanged)

private:
    QVector<ReleaseItemModel*> m_Releases;
    QList<ReleaseModel> m_ApiReleases;
    ReleaseLoader* m_ReleaseLoader;
    QThread *m_LoadedReleaseThread;
    bool m_NeedUpdate;

    static void addRelease(QQmlListProperty<ReleaseItemModel>*, ReleaseItemModel*);
    static int releasesCount(QQmlListProperty<ReleaseItemModel>*);
    static ReleaseItemModel* release(QQmlListProperty<ReleaseItemModel>*, int);
    static void clearReleases(QQmlListProperty<ReleaseItemModel>*);

public:
    explicit ReleasesService(QObject *parent = nullptr);

    Q_INVOKABLE void fillNextReleases();
    Q_INVOKABLE void loadReleasesCache();
    Q_INVOKABLE ReleaseItemModel* getRelease(int releaseId);

    QQmlListProperty<ReleaseItemModel> releases();
    void addRelease(ReleaseItemModel*);
    int releasesCount() const;
    ReleaseItemModel* release(int) const;
    void clearReleases();

signals:
    void releasesChanged();

public slots:
    void loadedReleases();
    void loadedReleasesFinished();
};

#endif // RELEASESSERVICE_H
