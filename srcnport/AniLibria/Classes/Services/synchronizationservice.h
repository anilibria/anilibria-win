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

private:
    AnilibriaApiService* m_AnilibriaApiService;
public:    
    explicit SynchronizationService(QObject *parent = nullptr);

    Q_INVOKABLE void synchronizeReleases();

signals:
    void synchronizationCompleted();
    void synchronizationFailed();

public slots:
    void saveReleasesToCache(QString data);
};

#endif // SYNCHRONIZATIONSERVICE_H
