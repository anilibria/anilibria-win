#include <QtCore>
#include <QQmlListProperty>
#include "synchronizationservice.h"
#include "../Models/releasemodel.h"

SynchronizationService::SynchronizationService(QObject *parent) : QObject(parent)
{
    m_AnilibriaApiService = new AnilibriaApiService(this);
    connect(m_AnilibriaApiService,&AnilibriaApiService::allReleasesReceived,this,&SynchronizationService::saveReleasesToCache);
    connect(m_AnilibriaApiService,&AnilibriaApiService::scheduleReceived,this,&SynchronizationService::saveScheduleToCache);
    connect(m_AnilibriaApiService,&AnilibriaApiService::signinReceived,this,&SynchronizationService::handleSignin);
    connect(m_AnilibriaApiService,&AnilibriaApiService::signoutReceived,this,&SynchronizationService::handleSignout);
}

void SynchronizationService::synchronizeReleases()
{
    m_AnilibriaApiService->getAllReleases();
}

void SynchronizationService::synchronizeSchedule()
{
    m_AnilibriaApiService->getSchedule();
}

void SynchronizationService::authorize(QString email, QString password, QString fa2code)
{
    m_AnilibriaApiService->signin(email, password, fa2code);
}

void SynchronizationService::saveReleasesToCache(QString data)
{
    emit synchronizedReleases(data);
}

void SynchronizationService::saveScheduleToCache(QString data)
{
    emit synchronizedSchedule(data);
}

void SynchronizationService::handleSignin(QString data)
{
    QJsonParseError jsonError;
    QJsonDocument jsonDocument = QJsonDocument::fromJson(data.toUtf8(), &jsonError);
    auto object = jsonDocument.object();
    auto token = object.value("token").toString();
    auto errorMessage = object.value("errorMessage").toString();
    if (errorMessage.isEmpty()) {
        emit userCompleteAuthontificated(token);
    } else {
        emit userFailedAuthontificated(errorMessage);
    }
}

void SynchronizationService::handleSignout()
{
    emit userSignouted();
}
