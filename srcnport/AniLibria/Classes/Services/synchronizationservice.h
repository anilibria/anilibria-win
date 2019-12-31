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
    Q_INVOKABLE void signout(QString token);
    Q_INVOKABLE void getUserData(QString token);
signals:
    void synchronizedReleases(QString data);
    void synchronizationCompleted();
    void synchronizationFailed();
    void synchronizedSchedule(QString data);
    void userCompleteAuthentificated(QString token);
    void userFailedAuthentificated(QString errorMessage);
    void userSignouted();
    void userDataReceived(QString data);

public slots:
    void saveReleasesToCache(QString data);
    void saveScheduleToCache(QString data);
    void handleSignin(QString data);
    void handleSignout();
    void handleUserData(QString data);
};

#endif // SYNCHRONIZATIONSERVICE_H
