#ifndef SYNCHRONIZATIONSERVICE_H
#define SYNCHRONIZATIONSERVICE_H

#include <QQmlListProperty>
#include <QObject>
#include "anilibriaapiservice.h"
#include "../Models/releasemodel.h"

class SynchronizationService : public QObject
{
    Q_OBJECT

private:
    AnilibriaApiService* m_AnilibriaApiService;
public:    
    explicit SynchronizationService(QObject *parent = nullptr);

    Q_INVOKABLE void synchronizeReleases();
    Q_INVOKABLE void synchronizeSchedule();
    Q_INVOKABLE void authorize(QString email, QString password, QString fa2code);
signals:
    void synchronizedReleases(QString data);
    void synchronizationCompleted();
    void synchronizationFailed();
    void synchronizedSchedule(QString data);
    void userCompleteAuthontificated(QString token);
    void userFailedAuthontificated(QString errorMessage);
    void userSignouted();

public slots:
    void saveReleasesToCache(QString data);
    void saveScheduleToCache(QString data);
    void handleSignin(QString data);
    void handleSignout();
};

#endif // SYNCHRONIZATIONSERVICE_H
