#ifndef SYNCHRONIZATIONSERVICE_H
#define SYNCHRONIZATIONSERVICE_H

#include <QQmlListProperty>
#include <QObject>
#include "anilibriaapiservice.h"
#include "../Models/releasemodel.h"
#include "../Models/releaseitemmodel.h"

class SynchronizationService : public QObject
{
    Q_OBJECT

    Q_PROPERTY(QQmlListProperty<ReleaseItemModel> releases READ releases NOTIFY releasesChanged)

private:
    AnilibriaApiService* m_AnilibriaApiService;
    QList<ReleaseModel> m_ApiReleases;
    QVector<ReleaseItemModel*> m_Releases;

    static void addRelease(QQmlListProperty<ReleaseItemModel>*, ReleaseItemModel*);
    static int releasesCount(QQmlListProperty<ReleaseItemModel>*);
    static ReleaseItemModel* release(QQmlListProperty<ReleaseItemModel>*, int);
    static void clearReleases(QQmlListProperty<ReleaseItemModel>*);
public:    
    explicit SynchronizationService(QObject *parent = nullptr);

    Q_INVOKABLE void synchronizeReleases();

    QQmlListProperty<ReleaseItemModel> releases();
    void addRelease(ReleaseItemModel*);
    int releasesCount() const;
    ReleaseItemModel* release(int) const;
    void clearReleases();    

signals:
    void releasesChanged();

public slots:
    void saveReleasesToCache(QString data);
};

#endif // SYNCHRONIZATIONSERVICE_H
